using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Log2Window.Properties;

using ControlExtenders;

using Log2Window.Log;
using Log2Window.Receiver;
using Log2Window.Settings;
using Log2Window.UI;

using Timer = System.Threading.Timer;
using System.Threading;
using System.Security.Permissions;
using System.Reflection;

namespace Log2Window
{
    public partial class MainForm : Form, ILogMessageNotifiable
    {
        private readonly bool _firstStartup;
        private readonly bool _isWin7orLater;
        private readonly WindowRestorer _windowRestorer;

        private readonly DockExtender _dockExtender;
        private readonly IFloaty _logDetailsPanelFloaty;
        private readonly IFloaty _loggersPanelFloaty;

        private string _msgDetailText = String.Empty;
        private bool _pauseLog;

        private Timer _taskbarProgressTimer;
        private const int _taskbarProgressTimerPeriod = 2000;
        private bool _addedLogMessage;

        private readonly Queue<LogMessage> _eventQueue;
        private Thread _logMsgThread;

        delegate void NotifyLogMsgCallback(LogMessage logMsg);
        delegate void NotifyLogMsgsCallback(LogMessage[] logMsgs);

        // Specific event handler on minimized action
        public event EventHandler Minimized;

        List<ToolStripButton> btnLogLevels = new List<ToolStripButton>();


        public MainForm()
        {
            Instance = this;

            InitializeComponent();

            btnTrace.Tag = LogLevel.Trace;
            btnLogLevels.Add(btnTrace);
            btnDebug.Tag = LogLevel.Debug;
            btnLogLevels.Add(btnDebug);
            btnInfo.Tag = LogLevel.Info;
            btnLogLevels.Add(btnInfo);
            btnWarn.Tag = LogLevel.Warn;
            btnLogLevels.Add(btnWarn);
            btnError.Tag = LogLevel.Error;
            btnLogLevels.Add(btnError);
            btnFatal.Tag = LogLevel.Fatal;
            btnLogLevels.Add(btnFatal);

            appNotifyIcon.Text = AboutForm.AssemblyTitle;

            //levelComboBox.SelectedIndex = 0;

            Minimized += OnMinimized;

            // Init Log Manager Singleton
            LogManager.Instance.Initialize(new TreeViewLoggerView(loggerTreeView), logListView);
            logListView.VirtualMode = true;
            logListView.RetrieveVirtualItem += LogListView_RetrieveVirtualItem;
            logListView.CacheVirtualItems += LogListView_CacheVirtualItems;
            logListView.SearchForVirtualItem += LogListView_SearchForVirtualItem;

            _dockExtender = new DockExtender(this);

            // Dockable Log Detail View
            _logDetailsPanelFloaty = _dockExtender.Attach(logDetailPanel, logDetailToolStrip, logDetailSplitter);
            _logDetailsPanelFloaty.DontHideHandle = true;
            _logDetailsPanelFloaty.Docking += OnFloatyDocking;

            // Dockable Logger Tree
            _loggersPanelFloaty = _dockExtender.Attach(loggerPanel, loggersToolStrip, loggerSplitter);
            _loggersPanelFloaty.DontHideHandle = true;
            _loggersPanelFloaty.Docking += OnFloatyDocking;

            // Settings
            _firstStartup = !UserSettings.Load();
            if (_firstStartup)
            {
                // Initialize default layout
                UserSettings.Instance.Layout.Set(DesktopBounds, WindowState, logDetailPanel, loggerPanel);

                // Force panel to visible
                UserSettings.Instance.Layout.ShowLogDetailView = true;
                UserSettings.Instance.Layout.ShowLoggerTree = true;
                UserSettings.Instance.DefaultFont = Environment.OSVersion.Version.Major >= 6 ? new Font("Segoe UI", 9F) : new Font("Tahoma", 8.25F);
            }

            Font = UserSettings.Instance.DefaultFont ?? Font;

            _windowRestorer = new WindowRestorer(this, UserSettings.Instance.Layout.WindowPosition,
                                                       UserSettings.Instance.Layout.WindowState);



            ApplySettings(true);

            _eventQueue = new Queue<LogMessage>();

            // Initialize Receivers
            foreach (IReceiver receiver in UserSettings.Instance.Receivers)
                InitializeReceiver(receiver);

            // Start the thread to process event logs in batch mode
            // Don't use timer, thread can ensure that only one thread comsume message to make message sequence is right.
            // When lots of message is waiting proccing, and one timer peroid is not enouth. timer may start more than one thread.
            // For example:
            //  System.Threading.Timer timer = new System.Threading.Timer(new TimerCallback(delegate (object ob)
            //  {
            //      System.Diagnostics.Utils.log.Debug("ManagedThreadId=" + Thread.CurrentThread.ManagedThreadId);
            //      Thread.Sleep(5000);
            //  }), null, 1000, 1000);

            _logMsgThread = new Thread(ProcessLogMessageThread);
            _logMsgThread.Start();
        }

        private void LogListView_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {

        }

        private void LogListView_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {

        }

        private void LogListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            lock (LogManager.Instance.dataLocker)
            {
                if (e.ItemIndex < LogManager.Instance._dataSource.Count)
                    e.Item = LogMessageItem.CreateListViewItem(LogManager.Instance._dataSource[e.ItemIndex].Message);
                else
                    Utils.log.Debug("LogListView_RetrieveVirtualItem: e.ItemIndex=" + e.ItemIndex + " _dataSource.Count=" + LogManager.Instance._dataSource.Count);
            }
        }
        private const int WM_SIZE = 0x0005;
        private const int SIZE_MINIMIZED = 1;
        internal static MainForm Instance;

        /// <summary>
        /// Catch on minimize event
        /// @author : Asbjørn Ulsberg -=|=- asbjornu@hotmail.com
        /// </summary>
        /// <param name="msg"></param>
        protected override void WndProc(ref Message msg)
        {

            if ((msg.Msg == WM_SIZE)
                && ((int)msg.WParam == SIZE_MINIMIZED)
                && (Minimized != null))
            {
                Minimized(this, EventArgs.Empty);
            }

            base.WndProc(ref msg);
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            if (_windowRestorer != null)
                _windowRestorer.TrackWindow();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_windowRestorer != null)
                _windowRestorer.TrackWindow();
        }

        protected override void OnShown(EventArgs e)
        {
            if (_firstStartup)
            {
                MessageBox.Show(
                    this,
                    @"Welcome to Log2Window! You must configure some Receivers in order to use the tool.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                ShowReceiversForm();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (_logMsgThread != null)
                {
                    _logMsgThread.Abort();
                    _logMsgThread = null;
                }

                if (_taskbarProgressTimer != null)
                {
                    _taskbarProgressTimer.Dispose();
                    _taskbarProgressTimer = null;
                }

                if ((UserSettings.Instance.Layout.LogListViewColumnsWidths == null) ||
                    (UserSettings.Instance.Layout.LogListViewColumnsWidths.Length != logListView.Columns.Count))
                {
                    UserSettings.Instance.Layout.LogListViewColumnsWidths = new int[logListView.Columns.Count];
                }

                for (int i = 0; i < logListView.Columns.Count; i++)
                {
                    UserSettings.Instance.Layout.LogListViewColumnsWidths[i] = logListView.Columns[i].Width;
                }

                UserSettings.Instance.Layout.Set(
                    _windowRestorer.WindowPosition, _windowRestorer.WindowState, logDetailPanel, loggerPanel);

                UserSettings.Instance.Save();
                UserSettings.Instance.Close();
            }
            catch (Exception ex)
            {
                Utils.log.Error(ex.Message, ex);
            }
        }

        protected override void OnLoad(EventArgs e)
        { 
            DoubleBuffered = true;
            base.OnLoad(e);
        }

        private void OnFloatyDocking(object sender, EventArgs e)
        {
            // make sure the ZOrder remains intact
            logListView.BringToFront();
            BringToFront();
        }

        private void ApplySettings(bool noCheck)
        {
            Opacity = (double)UserSettings.Instance.Transparency / 100;
            ShowInTaskbar = !UserSettings.Instance.HideTaskbarIcon;

            TopMost = UserSettings.Instance.AlwaysOnTop;
            pinOnTopBtn.Checked = UserSettings.Instance.AlwaysOnTop;
            pauseRefreshNewMessagesBtn.Checked = LogManager.Instance.PauseRefreshNewMessages;

            logListView.Font = UserSettings.Instance.LogListFont;
            logDetailTextBox.Font = UserSettings.Instance.LogDetailFont;
            loggerTreeView.Font = UserSettings.Instance.LoggerTreeFont;

            logListView.BackColor = UserSettings.Instance.LogListBackColor;
            logDetailTextBox.BackColor = UserSettings.Instance.LogMessageBackColor;
            tbMessage.BackColor = UserSettings.Instance.LogMessageBackColor;

            LogLevels.Instance.LogLevelInfos[(int)LogLevel.Trace].Color = UserSettings.Instance.TraceLevelColor;
            LogLevels.Instance.LogLevelInfos[(int)LogLevel.Debug].Color = UserSettings.Instance.DebugLevelColor;
            LogLevels.Instance.LogLevelInfos[(int)LogLevel.Info].Color = UserSettings.Instance.InfoLevelColor;
            LogLevels.Instance.LogLevelInfos[(int)LogLevel.Warn].Color = UserSettings.Instance.WarnLevelColor;
            LogLevels.Instance.LogLevelInfos[(int)LogLevel.Error].Color = UserSettings.Instance.ErrorLevelColor;
            LogLevels.Instance.LogLevelInfos[(int)LogLevel.Fatal].Color = UserSettings.Instance.FatalLevelColor;

            SetLogLevelBtnState();
            // gray like a disabled button. so leave it color as black.
            //btnTrace.ForeColor = UserSettings.Instance.TraceLevelColor;
            btnDebug.ForeColor = UserSettings.Instance.DebugLevelColor;
            btnInfo.ForeColor = UserSettings.Instance.InfoLevelColor;
            btnWarn.ForeColor = UserSettings.Instance.WarnLevelColor;
            btnError.ForeColor = UserSettings.Instance.ErrorLevelColor;
            btnFatal.ForeColor = UserSettings.Instance.FatalLevelColor;

            //See if the Columns Changed
            bool columnsChanged = false;

            if (logListView.Columns.Count != UserSettings.Instance.ColumnConfiguration.Length)
                columnsChanged = true;
            else
                for (int i = 0; i < UserSettings.Instance.ColumnConfiguration.Length; i++)
                {
                    if (!UserSettings.Instance.ColumnConfiguration[i].Name.Equals(logListView.Columns[i].Text))
                    {
                        columnsChanged = true;
                        break;
                    }
                }

            if (columnsChanged)
            {
                logListView.Columns.Clear();
                foreach (var column in UserSettings.Instance.ColumnConfiguration)
                {
                    logListView.Columns.Add(column.Name);
                }
            }

            // Layout
            if (noCheck)
            {
                DesktopBounds = UserSettings.Instance.Layout.WindowPosition;
                WindowState = UserSettings.Instance.Layout.WindowState;
 
                logDetailPanel.Size = UserSettings.Instance.Layout.LogDetailViewSize;                
                loggerPanel.Size = UserSettings.Instance.Layout.LoggerTreeSize;

                if (UserSettings.Instance.Layout.LogListViewColumnsWidths != null)
                {
                    for (int i = 0; i < UserSettings.Instance.Layout.LogListViewColumnsWidths.Length; i++)
                    {
                        if (i < logListView.Columns.Count)
                            logListView.Columns[i].Width = UserSettings.Instance.Layout.LogListViewColumnsWidths[i];
                    }
                }
            }
        }

        private void InitializeReceiver(IReceiver receiver)
        {
            try
            {
                receiver.Attach(this);
                receiver.Initialize();
                receiver.Start();

                //LogManager.Instance.SetRootLoggerName(String.Format("Root [{0}]", receiver));
            }
            catch (Exception ex)
            {
                Utils.log.Error(ex.Message, ex);
                try
                {
                    receiver.Terminate();
                }
                catch (Exception ex2)
                {
                    Utils.log.Error(ex2.Message, ex2);
                }

                ShowErrorBox("Failed to Initialize Receiver: " + ex.Message);
            }
        }

        private void TerminateReceiver(IReceiver receiver)
        {
            try
            {
                receiver.Stop();
                receiver.Terminate();
            }
            catch (Exception ex)
            {
                Utils.log.Error(ex.Message, ex);
                ShowErrorBox("Failed to Terminate Receiver: " + ex.Message);
            }
        }

        private void Quit()
        {
            Close();
        }

        private void ClearLogMessages()
        {
            ClearLogMessageDetail();
            LogManager.Instance.ClearLogMessages();
        }

        private void ClearLoggers()
        {
            ClearLogMessageDetail();
            LogManager.Instance.ClearAll();
        }

        private void CollapseLoggers()
        {
            loggerTreeView.CollapseAll();
            loggerTreeView.TopNode.Expand();
        }

        private void ClearAll()
        {
            ClearLogMessages();
            ClearLoggers();
        }

        protected void ShowBalloonTip(string msg)
        {
            appNotifyIcon.BalloonTipTitle = AboutForm.AssemblyTitle;
            appNotifyIcon.BalloonTipText = msg;
            appNotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            appNotifyIcon.ShowBalloonTip(3000);
        }

        private void ShowErrorBox(string msg)
        {
            MessageBox.Show(this, msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowSettingsForm()
        {
            // Make a copy of the settings in case the user cancels.
            UserSettings copy = UserSettings.Instance.Clone();
            SettingsForm form = new SettingsForm(copy);
            if (form.ShowDialog(this) != DialogResult.OK)
                return;

            UserSettings.Instance = copy;
            UserSettings.Instance.Save();

            ApplySettings(false);
        }

        private void ShowReceiversForm()
        {
            ReceiversForm form = new ReceiversForm(UserSettings.Instance.Receivers);
            if (form.ShowDialog(this) != DialogResult.OK)
                return;

            foreach (IReceiver receiver in form.RemovedReceivers)
            {
                TerminateReceiver(receiver);
                UserSettings.Instance.Receivers.Remove(receiver);
            }

            foreach (IReceiver receiver in form.AddedReceivers)
            {
                UserSettings.Instance.Receivers.Add(receiver);
                InitializeReceiver(receiver);
            }

            foreach (IReceiver receiver in form.ModifiedReceivers)
            {
                receiver.Terminate();
                UserSettings.Instance.Receivers.Remove(receiver);

                var newReceiver = (receiver as ICloneable).Clone() as IReceiver;
                UserSettings.Instance.Receivers.Add(newReceiver);
                InitializeReceiver(newReceiver);
            }

            UserSettings.Instance.Save();
        }


        private void ShowAboutForm()
        {
            AboutForm aboutBox = new AboutForm();
            aboutBox.ShowDialog(this);
        }

        private void RestoreWindow()
        {
            // Make the form visible and activate it. We need to bring the form
            // the front so the user will see it. Otherwise the user would have
            // to find it in the task bar and click on it.

            Visible = true;
            Activate();
            BringToFront();

            if (WindowState == FormWindowState.Minimized)
                WindowState = _windowRestorer.WindowState;
        }

        #region ILogMessageNotifiable Members

        /// <summary>
        /// Transforms the notification into an asynchronous call.
        /// The actual method called to add log messages is 'AddLogMessages'.
        /// </summary>
        public void Notify(LogMessage[] logMsgs)
        {
            //// InvokeRequired required compares the thread ID of the
            //// calling thread to the thread ID of the creating thread.
            //// If these threads are different, it returns true.
            //if (logListView.InvokeRequired)
            //{
            //    NotifyLogMsgsCallback d = AddLogMessages;
            //    Invoke(d, new object[] { logMsgs });
            //}
            //else
            //{
            //    AddLogMessages(logMsgs);
            //}

            lock (_eventQueue)
            {
                foreach (var logMessage in logMsgs)
                {
                    _eventQueue.Enqueue(logMessage);
                }
            }
        }

        /// <summary>
        /// Transforms the notification into an asynchronous call.
        /// The actual method called to add a log message is 'AddLogMessage'.
        /// </summary>
        public void Notify(LogMessage logMsg)
        {
            //// InvokeRequired required compares the thread ID of the
            //// calling thread to the thread ID of the creating thread.
            //// If these threads are different, it returns true.
            //if (logListView.InvokeRequired)
            //{
            //    NotifyLogMsgCallback d = AddLogMessage;
            //    Invoke(d, new object[] { logMsg });
            //}
            //else
            //{
            //    AddLogMessage(logMsg);
            //}

            lock (_eventQueue)
            {
                _eventQueue.Enqueue(logMsg);
            }
        }

        #endregion

        /// <summary>
        /// Adds a new log message, synchronously.
        /// </summary>
        private void AddLogMessages(IEnumerable<LogMessage> logMsgs)
        {
            if (_pauseLog)
                return;


            foreach (LogMessage msg in logMsgs)
                AddLogMessage(msg);

        }

        /// <summary>
        /// Adds a new log message, synchronously.
        /// </summary>
        private void AddLogMessage(LogMessage logMsg)
        {
            try
            {
                if (_pauseLog)
                    return;

                _addedLogMessage = true;

                LogManager.Instance.ProcessLogMessage(logMsg);

                if (!Visible && UserSettings.Instance.NotifyNewLogWhenHidden)
                    ShowBalloonTip("A new message has been received...");
            }
            catch (Exception ex)
            {
                Utils.log.Error(ex.Message, ex);
            }
        }

        private void ProcessLogMessageThread()
        {
            while (_logMsgThread != null)
            {
                try
                {
                    LogMessage[] messages;

                    lock (_eventQueue)
                    {
                        // Do a local copy to minimize the lock
                        messages = _eventQueue.ToArray();
                        _eventQueue.Clear();
                    }

                    // Process logs if any
                    if (messages.Length > 0)
                    {
                        AddLogMessages(messages);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }

                    if (_logMsgThread != null)
                    {
                        LoggerItem.TryEnsureVisibleLastItem(logListView);
                    }
                }
                catch (ThreadAbortException ex)
                {
                    Utils.log.Error("ProcessLogMessageThread " + ex.Message);
                    Thread.ResetAbort();
                    break;
                }
                catch (Exception ex)
                {
                    Utils.log.Error(ex.Message, ex);
                }
            }
        }

        private void quitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Quit();
            }
            catch (Exception ex)
            {
                Utils.log.Error("quitBtn_Click " + ex.Message);
                Environment.Exit(0);
            }
        }

        private void logListView_SelectedIndexChanged(object sender, EventArgs e)
        {

            LogMessageItem logMsgItem = null;
            if (logListView.SelectedIndices.Count > 0)
                logMsgItem = LogManager.Instance._dataSource[logListView.SelectedIndices[0]];

            SetLogMessageDetail(logMsgItem);

        }

        private void ClearLogMessageDetail()
        {
            logDetailTextBox.Text = string.Empty;
            tbMessage.Text = "";
            PopulateExceptions(null);
            OpenSourceFile(null, 0);
        }

        private void SetLogMessageDetail(LogMessageItem logMsgItem)
        {
            // Store the text to avoid editing without settings the control
            // as readonly... kind of ugly trick...

            if (logMsgItem != null)
            {
                //StringBuilder sb = new StringBuilder();

                //sb.Append(logMsgItem.GetMessageDetails());

                logDetailTextBox.ForeColor = logMsgItem.Message.Level.Color;
                logMsgItem.GetMessageDetails(logDetailTextBox, tbMessage);

                if (UserSettings.Instance.ShowMsgDetailsProperties)
                {
                    // Append properties
                    foreach (KeyValuePair<string, string> kvp in logMsgItem.Message.Properties)
                        logDetailTextBox.AppendText(string.Format("{0} = {1}{2}", kvp.Key, kvp.Value, Environment.NewLine));
                }

                // Append exception
                tbExceptions.Text = string.Empty;
                if (UserSettings.Instance.ShowMsgDetailsException &&
                    !String.IsNullOrEmpty(logMsgItem.Message.ExceptionString))
                {
                    //sb.AppendLine(logMsgItem.Message.ExceptionString);            
                    if (!string.IsNullOrEmpty(logMsgItem.Message.ExceptionString))
                    {
                        PopulateExceptions(logMsgItem.Message.ExceptionString);
                    }
                }

                if (logMsgItem.Message.SourceFileLineNr.HasValue)
                    OpenSourceFile(logMsgItem.Message.SourceFileName, logMsgItem.Message.SourceFileLineNr.Value);
            }
        }

        private void OpenSourceFile(string fileName, uint line)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                textEditorSourceCode.Visible = false;
                lbFileName.Text = string.Empty;
                return;
            }

            textEditorSourceCode.Visible = true;
            try
            {
                if (!File.Exists(fileName))
                {
                    //If the file cannot be found, try to locate it using the source code mapping configuration
                    var mappedFile = TryToLocateSourceFile(fileName);
                    if (string.IsNullOrEmpty(mappedFile))
                    {
                        textEditorSourceCode.Visible = false;
                        lbFileName.Text = fileName + " not found...";
                        return;
                    }

                    if (!File.Exists(mappedFile))
                    {
                        textEditorSourceCode.Visible = false;
                        lbFileName.Text = mappedFile + " not found...";
                        return;
                    }

                    fileName = mappedFile;
                }

                if (line > 1)
                    line--;
                textEditorSourceCode.LoadFile(fileName);
                textEditorSourceCode.ActiveTextAreaControl.TextArea.Caret.Line = (int)line;
                textEditorSourceCode.ActiveTextAreaControl.TextArea.Caret.UpdateCaretPosition();
                lbFileName.Text = fileName + ":" + line;
            }
            catch (Exception ex)
            {
                Utils.log.Error(ex.Message, ex);
                MessageBox.Show(string.Format("Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace), "Error opening source file");
            }
        }

        private string TryToLocateSourceFile(string file)
        {
            if (UserSettings.Instance.SourceLocationMapConfiguration != null)
                foreach (var sourceMap in UserSettings.Instance.SourceLocationMapConfiguration)
                {
                    if (file.StartsWith(sourceMap.LogSource))
                    {
                        file = sourceMap.LocalSource + file.Remove(0, sourceMap.LogSource.Length);
                        return file;
                    }
                }
            return null;
        }

        private void PopulateExceptions(string exceptions)
        {
            if (string.IsNullOrEmpty(exceptions))
            {
                tbExceptions.Text = string.Empty;
                return;
            }

            string[] lines = exceptions.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (!ParseCSharpStackTraceLine(line))
                {
                    //No supported exception stack traces is detected
                    tbExceptions.SelectedText = line;
                }
                //else if (Add other Parsers Here...)

                tbExceptions.SelectedText = "\r\n";
            }
        }

        private bool ParseCSharpStackTraceLine(string line)
        {
            bool stackTraceFileDetected = false;

            //Detect a C Sharp File                
            int endOfFileIndex = line.ToLower().LastIndexOf(".cs");
            if (endOfFileIndex != -1)
            {
                var leftTruncatedFile = line.Substring(0, endOfFileIndex + 3);
                int startOfFileIndex = leftTruncatedFile.LastIndexOf(":") - 1;
                if (startOfFileIndex >= 0)
                {
                    string fileName = leftTruncatedFile.Substring(startOfFileIndex, leftTruncatedFile.Length - startOfFileIndex);

                    const string lineSignature = ":line ";
                    int lineIndex = line.ToLower().LastIndexOf(lineSignature);
                    if (lineIndex != -1)
                    {
                        int lineSignatureLength = lineSignature.Length;
                        var lineNrString = line.Substring(lineIndex + lineSignatureLength,
                                                            line.Length - lineIndex - lineSignatureLength);
                        lineNrString = lineNrString.TrimEnd(new[] { ',' });
                        if (!string.IsNullOrEmpty(lineNrString))
                        {
                            uint parsedLineNr;
                            if (uint.TryParse(lineNrString, out parsedLineNr))
                            {
                                int fileLine = (int)parsedLineNr;
                                stackTraceFileDetected = true;

                                tbExceptions.SelectedText = line.Substring(0, startOfFileIndex - 1) + " ";
                                tbExceptions.InsertLink(string.Format("{0} line:{1}",
                                                                fileName, fileLine));
                            }
                        }
                    }
                }
            }

            return stackTraceFileDetected;
        }



        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearLogMessages();
        } 
        

        private void clearLoggersBtn_Click(object sender, EventArgs e)
        {
            ClearLoggers();
        }

        private void collapseAllBtn_Click(object sender, EventArgs e)
        {
            CollapseLoggers();
        } 

        private void copyLogDetailBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbMessage.Text))
                return;

            Clipboard.SetText(tbMessage.Text);
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            ShowAboutForm();
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
        }

        private void receiversBtn_Click(object sender, EventArgs e)
        {
            ShowReceiversForm();
        }

        private void appNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreWindow();
        }

        private void OnMinimized(object sender, EventArgs e)
        {
            if (!ShowInTaskbar)
                Visible = false;
        }

        private void restoreTrayMenuItem_Click(object sender, EventArgs e)
        {
            RestoreWindow();
        }

        private void settingsTrayMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
        }

        private void aboutTrayMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutForm();
        }

        private void exitTrayMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Quit();
            }
            catch (Exception ex)
            {
                Utils.log.Error("exitTrayMenuItem_Click " + ex.Message);
                Environment.Exit(0);
            }
        }

        private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Return || e.Alt || e.Control)
                return;
            using (new AutoWaitCursor())
            {
                try
                {
                    LogManager.Instance.SearchText(searchTextBox.Text);
                }
                finally
                {
                    ReBindListViewFromAllLogMessageItems();
                }
            }

        }

        private void zoomOutLogListBtn_Click(object sender, EventArgs e)
        {
            ZoomControlFont(logListView, false);
        }

        private void zoomInLogListBtn_Click(object sender, EventArgs e)
        {
            ZoomControlFont(logListView, true);
        }

        private void zoomOutLogDetailsBtn_Click(object sender, EventArgs e)
        {
            ZoomControlFont(logDetailTextBox, false);
        }

        private void zoomInLogDetailsBtn_Click(object sender, EventArgs e)
        {
            ZoomControlFont(logDetailTextBox, true);
        }

        private void pinOnTopBtn_Click(object sender, EventArgs e)
        {
            // Toggle check state
            pinOnTopBtn.Checked = !pinOnTopBtn.Checked;

            // Save and apply setting
            UserSettings.Instance.AlwaysOnTop = pinOnTopBtn.Checked;
            TopMost = pinOnTopBtn.Checked;
        }

        private static void ZoomControlFont(Control ctrl, bool zoomIn)
        {
            // Limit to a minimum size
            float newSize = Math.Max(0.5f, ctrl.Font.SizeInPoints + (zoomIn ? +1 : -1));
            ctrl.Font = new Font(ctrl.Font.FontFamily, newSize);
        }


        private void deleteLoggerTreeMenuItem_Click(object sender, EventArgs e)
        {
            LoggerItem logger = (LoggerItem)loggerTreeView.SelectedNode.Tag;

            if (logger != null)
            {
                logger.Remove();
            }

            ReBindListViewFromAllLogMessageItems(needDeleteMessages: true);
        }

        private void deleteAllLoggerTreeMenuItem_Click(object sender, EventArgs e)
        {
            ClearAll();

            ReBindListViewFromAllLogMessageItems(needDeleteMessages: true);
        }

        private void loggerTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                loggerTreeView.SelectedNode = loggerTreeView.GetNodeAt(e.X, e.Y);

                deleteLoggerTreeMenuItem.Enabled = (loggerTreeView.SelectedNode != null);

                loggerTreeContextMenuStrip.Show(loggerTreeView, e.Location);
            }
        }

        private void LoggerTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            //Utils.log.Debug("LoggerTreeView_BeforeCheck " + e.Action + " " + this.Name);
            //if (e.Action != TreeViewAction.ByMouse && e.Action != TreeViewAction.ByKeyboard)
            //    return;
        }


        private void loggerTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.ByMouse && e.Action != TreeViewAction.ByKeyboard)
                return;

            //Utils.log.Debug("loggerTreeView_AfterCheck " + e.Action + " " + this.Name);

            using (new AutoWaitCursor())
            {
                try
                {
                    // Enable/disable the logger item that is represented by the checked node.
                    (e.Node.Tag as LoggerItem).Enabled = e.Node.Checked;
                }
                finally
                {
                    ReBindListViewFromAllLogMessageItems();
                }
            }


            e.Node.Checked = e.Node.Checked;
        }

        public void ReBindListViewFromAllLogMessageItems(bool needDeleteMessages = false)
        {
            lock (LogManager.Instance.dataLocker)
            {
                if (needDeleteMessages)
                {
                    MyList<LogMessageItem> tempAllLogMessageItems = new MyList<LogMessageItem>();
                    foreach (var item in LogManager.Instance._allLogMessageItems)
                    {
                        if (!item.Parent._messagesDeleted)
                            tempAllLogMessageItems.Enqueue(item);
                    }
                    LogManager.Instance._allLogMessageItems.Clear();
                    LogManager.Instance._allLogMessageItems = tempAllLogMessageItems;
                }

                LogManager.Instance._dataSource.Clear();

                foreach (var item in LogManager.Instance._allLogMessageItems)
                {
                    item.Enabled = item.Parent.IsItemToBeEnabled(item);
                    if (item.Enabled)
                    {
                        LogManager.Instance._dataSource.Enqueue(item);
                    }
                }

                logListView.VirtualListSize = LogManager.Instance._dataSource.Count;

                //if (LogManager.Instance.PauseRefreshNewMessages)
                //{ 

                if (LogManager.Instance._dataSource.Count > 0)
                {
                    var lastIndex = LogManager.Instance._dataSource.Count - 1;
                    var thisArrivedId = LogManager.Instance._dataSource[lastIndex].Message.ArrivedId;
                    LoggerItem.lastEnsureVisibleArrivedId = thisArrivedId;
                    logListView.EnsureVisible(lastIndex);
                    logListView.SelectedIndices.Clear();
                    logListView.SelectedIndices.Add(lastIndex);
                }

                this.RefreshTitle();
                //}
            }
        }


        private void LoggerTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //Utils.log.Debug("LoggerTreeView_NodeMouseClick "+ e.Node.Checked);

        }

        private void loggerTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void goToFirstLogBtn_Click(object sender, EventArgs e)
        {
            lock (LogManager.Instance.dataLocker)
            {
                if (LogManager.Instance._dataSource.Count == 0)
                    return;

                logListView.EnsureVisible(0);
                logListView.SelectedIndices.Clear();
                logListView.SelectedIndices.Add(0);
            }
        }

        private void goToLastLogBtn_Click(object sender, EventArgs e)
        {
            lock (LogManager.Instance.dataLocker)
            {
                if (LogManager.Instance._dataSource.Count == 0)
                    return;

                logListView.VirtualListSize = LogManager.Instance._dataSource.Count;
                logListView.EnsureVisible(LogManager.Instance._dataSource.Count - 1);
                logListView.SelectedIndices.Clear();
                logListView.SelectedIndices.Add(LogManager.Instance._dataSource.Count - 1);
            }
        }

        private void pauseRefreshNewMessagesBtn_Click(object sender, EventArgs e)
        {
            LogManager.Instance.PauseRefreshNewMessages = !LogManager.Instance.PauseRefreshNewMessages;

            pauseRefreshNewMessagesBtn.Checked = LogManager.Instance.PauseRefreshNewMessages;

            if (pauseRefreshNewMessagesBtn.Checked)
            {
                this.BackColor = Color.Red;
            }
            else
            {
                this.BackColor = SystemColors.InactiveBorder;
                ReBindListViewFromAllLogMessageItems();
            }
        }

        public void RefreshTitle()
        {
            StringBuilder sb = new StringBuilder("Log2Window");
            if (pauseRefreshNewMessagesBtn.Checked)
            {
                sb.Append(" - Pause");
            }
            else
            {
                sb.Append(" -      ");
            }

            sb.Append(" Count: " + LogManager.Instance._dataSource.Count + "/" + LogManager.Instance._allLogMessageItems.Count);

            this.Text = sb.ToString();
        }

        private void clearAll_Click(object sender, EventArgs e)
        {
            ClearAll();
        }


        /// <summary>
        /// Quick and dirty implementation of an export function...
        /// </summary>
        private void saveBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void saveToExcelBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void btnOpenFileInVS_Click(object sender, EventArgs e)
        {
            try
            {
                var processInfo = new ProcessStartInfo("devenv",
                                                       string.Format("/edit \"{0}\" /command \"Edit.Goto {1}\"",
                                                                     textEditorSourceCode.FileName, 0));
                var process = Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Utils.log.Error(ex.Message, ex);
                MessageBox.Show(ex.Message, "Error opening file in Visual Studio");
            }
        }

        private void TbExceptionsLinkClicked(object sender, LinkClickedEventArgs e)
        {
            string exception = e.LinkText;
            if (exception != null)
            {
                var exceptionPair = exception.Split(new[] { " line:" }, StringSplitOptions.None);
                if (exceptionPair.Length == 2)
                {
                    int lineNr;
                    int.TryParse(exceptionPair[1], out lineNr);

                    OpenSourceFile(exceptionPair[0], (uint)lineNr);
                    tabControlDetail.SelectedTab = tabSource;
                }
            }
        }

        private void quickLoadBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(openFileDialog1.FileName))
                {
                    MessageBox.Show(string.Format("File: {0} does not exists", openFileDialog1.FileName),
                                    "Error Opening Log File");
                    return;
                }

                FileReceiver fr = new FileReceiver();
                fr.FileToWatch = openFileDialog1.FileName;
                fr.ShowFromBeginning = true;
                InitializeReceiver(fr);
            }
        }

        EventLogReceiver eventLogReceiver; 

        private void logListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboard();
        }

        private void CopySelectedValuesToClipboard()
        {
            var builder = new StringBuilder();
            foreach (int index in logListView.SelectedIndices)
            {
                var logMsgItem = LogManager.Instance._dataSource[index];
                if (logMsgItem != null)
                    builder.AppendLine(logMsgItem.Message.ToString());
            }

            Clipboard.SetText(builder.ToString());
        }

        private void btnLogLevel_Click(object sender, EventArgs e)
        {
            if (!IsHandleCreated)
                return;

            using (new AutoWaitCursor())
            {
                var btn = (ToolStripButton)sender;
                UserSettings.Instance.LogLevelInfo =
                    LogUtils.GetLogLevelInfo((LogLevel)btn.Tag);

                SetLogLevelBtnState();

                try
                {
                    LogManager.Instance.UpdateLogLevel();
                }
                finally
                {
                    ReBindListViewFromAllLogMessageItems();
                }
            }
        }

        private void SetLogLevelBtnState()
        {
            foreach (var btLogLevel in btnLogLevels)
            {
                if ((LogLevel)btLogLevel.Tag >= UserSettings.Instance.LogLevelInfo.Level)
                {
                    btLogLevel.Checked = true;
                }
                else
                {
                    btLogLevel.Checked = false;
                }
            }
        }


        private void DdbEventLog_DropDownOpening(object sender, System.EventArgs e)
        {
            if (miLoadEventLog.DropDownItems.Count == 0)
            {
                var eventLogs = EventLog.GetEventLogs();

                {
                    ToolStripMenuItem menuItemAll = new ToolStripMenuItem("--All--");
                    miLoadEventLog.DropDownItems.Add(menuItemAll);
                    foreach (var item in eventLogs)
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(item.Log);
                        miLoadEventLog.DropDownItems.Add(menuItem);
                    }
                    foreach (ToolStripMenuItem item in miLoadEventLog.DropDownItems)
                    {
                        item.Click += miLoadEventLogSubutem_Click;
                    }
                }

                {
                    ToolStripMenuItem menuItemAll = new ToolStripMenuItem("--All--");
                    miClearEventLog.DropDownItems.Add(menuItemAll);
                    foreach (var item in eventLogs)
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(item.Log);
                        miClearEventLog.DropDownItems.Add(menuItem);
                    }
                    foreach (ToolStripMenuItem item in miClearEventLog.DropDownItems)
                    {
                        item.Click += miClearEventLogSubutem_Click;
                    }
                }

            }
        }



        private void miLoadEventLogSubutem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            if (eventLogReceiver != null)
            {
                try
                {
                    eventLogReceiver.Terminate();
                    eventLogReceiver = null;
                }
                catch (Exception ex)
                {
                    Utils.log.Error(ex.Message, ex);
                }

                if (menuItem.Text != "--All--")
                    MessageBox.Show("Only listen selected eventlog now.", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (menuItem.Text == "--All--")
            { 
                this.Cursor = Cursors.WaitCursor;
                eventLogReceiver = new EventLogReceiver();
                eventLogReceiver.ShowFromBeginning = true;
                InitializeReceiver(eventLogReceiver);
                this.Cursor = Cursors.Default;
            }
            else
            { 
                this.Cursor = Cursors.WaitCursor;
                eventLogReceiver = new EventLogReceiver();
                eventLogReceiver.LogName = menuItem.Text;
                eventLogReceiver.ShowFromBeginning = true;
                InitializeReceiver(eventLogReceiver);
                this.Cursor = Cursors.Default;
            }
        } 

        private void miClearEventLogSubutem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            //Request administrator rule to do it.
            var psi = new ProcessStartInfo();
            psi.FileName = Assembly.GetExecutingAssembly().Location;
            psi.Arguments = "ClearEventLog " + menuItem.Text;
            psi.Verb = "runas";

            var process = new Process();
            process.StartInfo = psi;
            process.Start();
            process.WaitForExit(); 
        }

        public static void ClearEventLog(string log)
        {
            var eventLogs = EventLog.GetEventLogs();
            if (log == "--All--")
            {
                foreach (var item in eventLogs)
                {
                    try
                    {
                        item.Clear();
                    }
                    catch (Exception ex)
                    {
                        Utils.log.Error(ex.Message, ex);
                    }
                }
            }
            else
            {
                foreach (var item in eventLogs)
                {
                    if (item.Log == log)
                    {
                        item.Clear();
                    }
                }
            }
        }

        private void lblSearch_Click(object sender, EventArgs e)
        {
            using (new AutoWaitCursor())
            {
                try
                {
                    LogManager.Instance.SearchText(searchTextBox.Text);
                }
                finally
                {
                    ReBindListViewFromAllLogMessageItems();
                }
            }
        }

        private void miExportLog4jXmlFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "log files (*.log)|*.log";
            dlg.FileName = "logs";
            dlg.Title = "Export to log4j xml file";
            if (dlg.ShowDialog(this) == DialogResult.Cancel)
                return;

            Utils.Export2Log4jFile(dlg.FileName);
        }

        private void miExportExcelCsvFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "csv files (*.csv)|*.csv";
            dlg.FileName = "logs";
            dlg.Title = "Export to csv file";
            if (dlg.ShowDialog(this) == DialogResult.Cancel)
                return;

            Utils.Export2Excel(dlg.FileName);
        }
    }
}
