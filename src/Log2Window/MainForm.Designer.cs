using Log2Window.UI;
using System.Windows.Forms;

namespace Log2Window
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.zoomOutLogListBtn = new System.Windows.Forms.ToolStripButton();
            this.zoomInLogListBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.goToFirstLogBtn = new System.Windows.Forms.ToolStripButton();
            this.pauseRefreshNewMessagesBtn = new System.Windows.Forms.ToolStripButton();
            this.goToLastLogBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.clearBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.btnFatal = new System.Windows.Forms.ToolStripButton();
            this.btnError = new System.Windows.Forms.ToolStripButton();
            this.btnWarn = new System.Windows.Forms.ToolStripButton();
            this.btnInfo = new System.Windows.Forms.ToolStripButton();
            this.btnDebug = new System.Windows.Forms.ToolStripButton();
            this.btnTrace = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lblSearch = new System.Windows.Forms.ToolStripLabel();
            this.searchTextBox = new MyToolStripTextBox();
            this.searchThreadBox = new MyToolStripTextBox();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsBtn = new System.Windows.Forms.ToolStripButton();
            this.receiversBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.ddbEventLog = new System.Windows.Forms.ToolStripDropDownButton();
            this.miLoadEventLog = new System.Windows.Forms.ToolStripMenuItem();
            this.miClearEventLog = new System.Windows.Forms.ToolStripMenuItem();
            this.ddbOpenLogFileBtn = new System.Windows.Forms.ToolStripDropDownButton();
            this.miOpenPatternLayoutFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenLog4jXmlFile = new System.Windows.Forms.ToolStripMenuItem();
            this.ddbExportBtn = new System.Windows.Forms.ToolStripDropDownButton();
            this.miExportLog4jXmlFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miExportExcelCsvFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.pinOnTopBtn = new System.Windows.Forms.ToolStripButton();
            this.timeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.levelColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loggerColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.threadColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.msgColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loggerPanel = new System.Windows.Forms.Panel();
            this.loggerInnerPanel = new System.Windows.Forms.Panel();
            this.loggersToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearLoggersBtn = new System.Windows.Forms.ToolStripButton();
            this.collapseAllBtn = new System.Windows.Forms.ToolStripButton();
            this.dactivateSourcesBtn = new System.Windows.Forms.ToolStripButton();
            this.keepHighlightBtn = new System.Windows.Forms.ToolStripButton();
            this.loggerTreeView = new Log2Window.UI.TreeViewWithoutDoubleClick();
            this.loggerSplitter = new System.Windows.Forms.Splitter();
            this.appNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreTrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsTrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutTrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.exitTrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logDetailPanel = new System.Windows.Forms.Panel();
            this.tabControlDetail = new System.Windows.Forms.TabControl();
            this.tabMessage = new System.Windows.Forms.TabPage();
            this.logDetailInnerPanel = new System.Windows.Forms.Panel();
            this.tbMessage = new System.Windows.Forms.RichTextBox();
            this.logDetailTextBox = new System.Windows.Forms.RichTextBox();
            this.logDetailToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomOutLogDetailsBtn = new System.Windows.Forms.ToolStripButton();
            this.zoomInLogDetailsBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.copyLogDetailBtn = new System.Windows.Forms.ToolStripButton();
            this.tabExceptions = new System.Windows.Forms.TabPage();
            this.tbExceptions = new RichTextBoxLinks.RichTextBoxEx();
            this.tabSource = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textEditorSourceCode = new ICSharpCode.TextEditor.TextEditorControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.lbFileName = new System.Windows.Forms.ToolStripLabel();
            this.btnOpenFileInVS = new System.Windows.Forms.ToolStripButton();
            this.logDetailSplitter = new System.Windows.Forms.Splitter();
            this.loggerTreeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteLoggerTreeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteAllLoggerTreeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.logListView = new Log2Window.UI.FlickerFreeListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainToolStrip.SuspendLayout();
            this.loggerPanel.SuspendLayout();
            this.loggerInnerPanel.SuspendLayout();
            this.loggersToolStrip.SuspendLayout();
            this.trayContextMenuStrip.SuspendLayout();
            this.logDetailPanel.SuspendLayout();
            this.tabControlDetail.SuspendLayout();
            this.tabMessage.SuspendLayout();
            this.logDetailInnerPanel.SuspendLayout();
            this.logDetailToolStrip.SuspendLayout();
            this.tabExceptions.SuspendLayout();
            this.tabSource.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.loggerTreeContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomOutLogListBtn,
            this.zoomInLogListBtn,
            this.toolStripSeparator15,
            this.goToFirstLogBtn,
            this.pauseRefreshNewMessagesBtn,
            this.goToLastLogBtn,
            this.toolStripSeparator13,
            this.clearBtn,
            this.toolStripSeparator10,
            this.toolStripLabel3,
            this.btnFatal,
            this.btnError,
            this.btnWarn,
            this.btnInfo,
            this.btnDebug,
            this.btnTrace,
            this.toolStripSeparator1,
            this.lblSearch,
            this.searchTextBox,
            this.searchThreadBox,
            this.toolStripSeparator9,
            this.settingsBtn,
            this.receiversBtn,
            this.toolStripSeparator14,
            this.ddbEventLog,
            this.ddbOpenLogFileBtn,
            this.ddbExportBtn,
            this.toolStripSeparator4,
            this.aboutBtn,
            this.toolStripSeparator12,
            this.pinOnTopBtn});
            this.mainToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.mainToolStrip.Size = new System.Drawing.Size(1363, 29);
            this.mainToolStrip.TabIndex = 2;
            this.mainToolStrip.Text = "mainToolStrip";
            // 
            // zoomOutLogListBtn
            // 
            this.zoomOutLogListBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutLogListBtn.Image = global::Log2Window.Properties.Resources.zoomout16;
            this.zoomOutLogListBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutLogListBtn.Name = "zoomOutLogListBtn";
            this.zoomOutLogListBtn.Size = new System.Drawing.Size(23, 20);
            this.zoomOutLogListBtn.ToolTipText = "Zoom Out Log List Font";
            this.zoomOutLogListBtn.Click += new System.EventHandler(this.zoomOutLogListBtn_Click);
            // 
            // zoomInLogListBtn
            // 
            this.zoomInLogListBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInLogListBtn.Image = global::Log2Window.Properties.Resources.zoomin16;
            this.zoomInLogListBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInLogListBtn.Name = "zoomInLogListBtn";
            this.zoomInLogListBtn.Size = new System.Drawing.Size(23, 20);
            this.zoomInLogListBtn.ToolTipText = "Zoom In Log List Font";
            this.zoomInLogListBtn.Click += new System.EventHandler(this.zoomInLogListBtn_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 23);
            // 
            // goToFirstLogBtn
            // 
            this.goToFirstLogBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.goToFirstLogBtn.Image = global::Log2Window.Properties.Resources.backward16;
            this.goToFirstLogBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goToFirstLogBtn.Name = "goToFirstLogBtn";
            this.goToFirstLogBtn.Size = new System.Drawing.Size(23, 20);
            this.goToFirstLogBtn.ToolTipText = "Go to First Log Message";
            this.goToFirstLogBtn.Click += new System.EventHandler(this.goToFirstLogBtn_Click);
            // 
            // pauseRefreshNewMessagesBtn
            // 
            this.pauseRefreshNewMessagesBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseRefreshNewMessagesBtn.Image = global::Log2Window.Properties.Resources.Pause16;
            this.pauseRefreshNewMessagesBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseRefreshNewMessagesBtn.Name = "pauseRefreshNewMessagesBtn";
            this.pauseRefreshNewMessagesBtn.Size = new System.Drawing.Size(23, 20);
            this.pauseRefreshNewMessagesBtn.ToolTipText = "Pause refresh new messages. (Messages still be processed in background.)";
            this.pauseRefreshNewMessagesBtn.Click += new System.EventHandler(this.pauseRefreshNewMessagesBtn_Click);
            // 
            // goToLastLogBtn
            // 
            this.goToLastLogBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.goToLastLogBtn.Image = global::Log2Window.Properties.Resources.forward16;
            this.goToLastLogBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goToLastLogBtn.Name = "goToLastLogBtn";
            this.goToLastLogBtn.Size = new System.Drawing.Size(23, 20);
            this.goToLastLogBtn.ToolTipText = "Go to Last Log Message";
            this.goToLastLogBtn.Click += new System.EventHandler(this.goToLastLogBtn_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 23);
            // 
            // clearBtn
            // 
            this.clearBtn.Image = global::Log2Window.Properties.Resources.deletefile16;
            this.clearBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(54, 20);
            this.clearBtn.Text = "Clear";
            this.clearBtn.ToolTipText = "Clear Log Messages";
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel3.Image = global::Log2Window.Properties.Resources.burn16;
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(16, 16);
            this.toolStripLabel3.ToolTipText = "Log Level Filter";
            // 
            // btnFatal
            // 
            this.btnFatal.AutoToolTip = false;
            this.btnFatal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFatal.Name = "btnFatal";
            this.btnFatal.Size = new System.Drawing.Size(36, 19);
            this.btnFatal.Text = "Fatal";
            this.btnFatal.Click += new System.EventHandler(this.btnLogLevel_Click);
            // 
            // btnError
            // 
            this.btnError.AutoToolTip = false;
            this.btnError.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnError.Name = "btnError";
            this.btnError.Size = new System.Drawing.Size(36, 19);
            this.btnError.Text = "Error";
            this.btnError.Click += new System.EventHandler(this.btnLogLevel_Click);
            // 
            // btnWarn
            // 
            this.btnWarn.AutoToolTip = false;
            this.btnWarn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWarn.Name = "btnWarn";
            this.btnWarn.Size = new System.Drawing.Size(39, 19);
            this.btnWarn.Text = "Warn";
            this.btnWarn.Click += new System.EventHandler(this.btnLogLevel_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.AutoToolTip = false;
            this.btnInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(32, 19);
            this.btnInfo.Text = "Info";
            this.btnInfo.Click += new System.EventHandler(this.btnLogLevel_Click);
            // 
            // btnDebug
            // 
            this.btnDebug.AutoToolTip = false;
            this.btnDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(46, 19);
            this.btnDebug.Text = "Debug";
            this.btnDebug.Click += new System.EventHandler(this.btnLogLevel_Click);
            // 
            // btnTrace
            // 
            this.btnTrace.AutoToolTip = false;
            this.btnTrace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTrace.Name = "btnTrace";
            this.btnTrace.Size = new System.Drawing.Size(38, 19);
            this.btnTrace.Text = "Trace";
            this.btnTrace.Click += new System.EventHandler(this.btnLogLevel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // lblSearch
            // 
            this.lblSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.lblSearch.Image = global::Log2Window.Properties.Resources.find16;
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(16, 16);
            this.lblSearch.Text = "toolStripLabel4";
            this.lblSearch.ToolTipText = "Search Text in Log Messages";
            this.lblSearch.Click += new System.EventHandler(this.lblSearch_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchTextBox.CueBanner = "Search Text";
            this.searchTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchTextBox.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(100, 23);
            this.searchTextBox.ToolTipText = "Search Text in Log Messages";
            this.searchTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.searchBox_KeyUp);
            // 
            // searchThreadBox
            // 
            this.searchThreadBox.BackColor = System.Drawing.SystemColors.Window;
            this.searchThreadBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchThreadBox.CueBanner = "Search Threads";
            this.searchThreadBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchThreadBox.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.searchThreadBox.Name = "searchThreadBox";
            this.searchThreadBox.Size = new System.Drawing.Size(100, 23);
            this.searchThreadBox.ToolTipText = "Search by Thread in Log Messages";
            this.searchThreadBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.searchBox_KeyUp);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 23);
            // 
            // settingsBtn
            // 
            this.settingsBtn.AutoToolTip = false;
            this.settingsBtn.Image = global::Log2Window.Properties.Resources.configure16;
            this.settingsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(78, 20);
            this.settingsBtn.Text = "Settings...";
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // receiversBtn
            // 
            this.receiversBtn.AutoToolTip = false;
            this.receiversBtn.Image = global::Log2Window.Properties.Resources.receive;
            this.receiversBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.receiversBtn.Name = "receiversBtn";
            this.receiversBtn.Size = new System.Drawing.Size(85, 20);
            this.receiversBtn.Text = "Receivers...";
            this.receiversBtn.Click += new System.EventHandler(this.receiversBtn_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 23);
            // 
            // ddbEventLog
            // 
            this.ddbEventLog.AutoToolTip = false;
            this.ddbEventLog.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLoadEventLog,
            this.miClearEventLog});
            this.ddbEventLog.Image = global::Log2Window.Properties.Resources.eventLog;
            this.ddbEventLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddbEventLog.Name = "ddbEventLog";
            this.ddbEventLog.Size = new System.Drawing.Size(85, 20);
            this.ddbEventLog.Text = "EventLog";
            this.ddbEventLog.Click += new System.EventHandler(this.DdbEventLog_DropDownOpening);
            // 
            // miLoadEventLog
            // 
            this.miLoadEventLog.Name = "miLoadEventLog";
            this.miLoadEventLog.Size = new System.Drawing.Size(101, 22);
            this.miLoadEventLog.Text = "Load";
            // 
            // miClearEventLog
            // 
            this.miClearEventLog.Name = "miClearEventLog";
            this.miClearEventLog.Size = new System.Drawing.Size(101, 22);
            this.miClearEventLog.Text = "Clear";
            // 
            // ddbOpenLogFileBtn
            // 
            this.ddbOpenLogFileBtn.AutoToolTip = false;
            this.ddbOpenLogFileBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOpenPatternLayoutFile,
            this.miOpenLog4jXmlFile});
            this.ddbOpenLogFileBtn.Image = global::Log2Window.Properties.Resources.documentsorcopy16;
            this.ddbOpenLogFileBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddbOpenLogFileBtn.Name = "ddbOpenLogFileBtn";
            this.ddbOpenLogFileBtn.Size = new System.Drawing.Size(103, 20);
            this.ddbOpenLogFileBtn.Text = "OpenLogFile";
            // 
            // miOpenPatternLayoutFile
            // 
            this.miOpenPatternLayoutFile.Name = "miOpenPatternLayoutFile";
            this.miOpenPatternLayoutFile.Size = new System.Drawing.Size(204, 22);
            this.miOpenPatternLayoutFile.Text = "Open Pattern Layout File";
            this.miOpenPatternLayoutFile.Click += new System.EventHandler(this.miOpenPatternLayoutFile_Click);
            // 
            // miOpenLog4jXmlFile
            // 
            this.miOpenLog4jXmlFile.Name = "miOpenLog4jXmlFile";
            this.miOpenLog4jXmlFile.Size = new System.Drawing.Size(204, 22);
            this.miOpenLog4jXmlFile.Text = "Open log4j xml file";
            this.miOpenLog4jXmlFile.Click += new System.EventHandler(this.miOpenLog4jXmlFile_Click);
            // 
            // ddbExportBtn
            // 
            this.ddbExportBtn.AutoToolTip = false;
            this.ddbExportBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExportLog4jXmlFile,
            this.miExportExcelCsvFile});
            this.ddbExportBtn.Image = global::Log2Window.Properties.Resources.saveas16;
            this.ddbExportBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddbExportBtn.Name = "ddbExportBtn";
            this.ddbExportBtn.Size = new System.Drawing.Size(70, 20);
            this.ddbExportBtn.Text = "Export";
            // 
            // miExportLog4jXmlFile
            // 
            this.miExportLog4jXmlFile.Image = global::Log2Window.Properties.Resources.saveas16;
            this.miExportLog4jXmlFile.Name = "miExportLog4jXmlFile";
            this.miExportLog4jXmlFile.Size = new System.Drawing.Size(193, 22);
            this.miExportLog4jXmlFile.Text = "Export to log4j xml file";
            this.miExportLog4jXmlFile.Click += new System.EventHandler(this.miExportLog4jXmlFile_Click);
            // 
            // miExportExcelCsvFile
            // 
            this.miExportExcelCsvFile.Image = global::Log2Window.Properties.Resources.Excel;
            this.miExportExcelCsvFile.Name = "miExportExcelCsvFile";
            this.miExportExcelCsvFile.Size = new System.Drawing.Size(193, 22);
            this.miExportExcelCsvFile.Text = "Export to excel csv file";
            this.miExportExcelCsvFile.Click += new System.EventHandler(this.miExportExcelCsvFile_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 23);
            // 
            // aboutBtn
            // 
            this.aboutBtn.AutoToolTip = false;
            this.aboutBtn.Image = global::Log2Window.Properties.Resources.infoabout16;
            this.aboutBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(60, 20);
            this.aboutBtn.Text = "About";
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 23);
            // 
            // pinOnTopBtn
            // 
            this.pinOnTopBtn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pinOnTopBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pinOnTopBtn.Image = global::Log2Window.Properties.Resources.cd16;
            this.pinOnTopBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pinOnTopBtn.Name = "pinOnTopBtn";
            this.pinOnTopBtn.Size = new System.Drawing.Size(23, 20);
            this.pinOnTopBtn.Text = "Pin on Top";
            this.pinOnTopBtn.Click += new System.EventHandler(this.pinOnTopBtn_Click);
            // 
            // timeColumnHeader
            // 
            this.timeColumnHeader.Text = "Time";
            this.timeColumnHeader.Width = 120;
            // 
            // levelColumnHeader
            // 
            this.levelColumnHeader.Text = "Level";
            this.levelColumnHeader.Width = 48;
            // 
            // loggerColumnHeader
            // 
            this.loggerColumnHeader.Text = "Logger";
            this.loggerColumnHeader.Width = 92;
            // 
            // threadColumnHeader
            // 
            this.threadColumnHeader.Text = "Thread";
            this.threadColumnHeader.Width = 52;
            // 
            // msgColumnHeader
            // 
            this.msgColumnHeader.Text = "Message";
            this.msgColumnHeader.Width = 751;
            // 
            // loggerPanel
            // 
            this.loggerPanel.Controls.Add(this.loggerInnerPanel);
            this.loggerPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.loggerPanel.Location = new System.Drawing.Point(1126, 29);
            this.loggerPanel.Name = "loggerPanel";
            this.loggerPanel.Size = new System.Drawing.Size(237, 547);
            this.loggerPanel.TabIndex = 5;
            // 
            // loggerInnerPanel
            // 
            this.loggerInnerPanel.Controls.Add(this.loggersToolStrip);
            this.loggerInnerPanel.Controls.Add(this.loggerTreeView);
            this.loggerInnerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loggerInnerPanel.Location = new System.Drawing.Point(0, 0);
            this.loggerInnerPanel.Name = "loggerInnerPanel";
            this.loggerInnerPanel.Size = new System.Drawing.Size(237, 547);
            this.loggerInnerPanel.TabIndex = 5;
            // 
            // loggersToolStrip
            // 
            this.loggersToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator2,
            this.clearLoggersBtn,
            this.collapseAllBtn,
            this.dactivateSourcesBtn,
            this.keepHighlightBtn});
            this.loggersToolStrip.Location = new System.Drawing.Point(0, 0);
            this.loggersToolStrip.Name = "loggersToolStrip";
            this.loggersToolStrip.Size = new System.Drawing.Size(237, 25);
            this.loggersToolStrip.TabIndex = 0;
            this.loggersToolStrip.Text = "Loggers";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(51, 22);
            this.toolStripLabel1.Text = "Loggers";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // clearLoggersBtn
            // 
            this.clearLoggersBtn.Image = global::Log2Window.Properties.Resources.delete16;
            this.clearLoggersBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearLoggersBtn.Name = "clearLoggersBtn";
            this.clearLoggersBtn.Size = new System.Drawing.Size(71, 22);
            this.clearLoggersBtn.Text = "Clear All";
            this.clearLoggersBtn.ToolTipText = "Clear All Loggers and Log Messages";
            this.clearLoggersBtn.Click += new System.EventHandler(this.clearLoggersBtn_Click);
            // 
            // collapseAllBtn
            // 
            this.collapseAllBtn.Image = global::Log2Window.Properties.Resources.collapse_all;
            this.collapseAllBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.collapseAllBtn.Name = "collapseAllBtn";
            this.collapseAllBtn.Size = new System.Drawing.Size(72, 22);
            this.collapseAllBtn.Text = "Collapse";
            this.collapseAllBtn.ToolTipText = "Collapse all sources";
            this.collapseAllBtn.Click += new System.EventHandler(this.collapseAllBtn_Click);
            // 
            // dactivateSourcesBtn
            // 
            this.dactivateSourcesBtn.Name = "dactivateSourcesBtn";
            this.dactivateSourcesBtn.Size = new System.Drawing.Size(23, 4);
            // 
            // keepHighlightBtn
            // 
            this.keepHighlightBtn.Name = "keepHighlightBtn";
            this.keepHighlightBtn.Size = new System.Drawing.Size(23, 4);
            // 
            // loggerTreeView
            // 
            this.loggerTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loggerTreeView.CheckBoxes = true;
            this.loggerTreeView.Indent = 19;
            this.loggerTreeView.Location = new System.Drawing.Point(0, 25);
            this.loggerTreeView.Name = "loggerTreeView";
            this.loggerTreeView.PathSeparator = ".";
            this.loggerTreeView.Size = new System.Drawing.Size(237, 522);
            this.loggerTreeView.TabIndex = 1;
            this.loggerTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.loggerTreeView_AfterCheck);
            this.loggerTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.loggerTreeView_AfterSelect);
            this.loggerTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.loggerTreeView_MouseUp);
            // 
            // loggerSplitter
            // 
            this.loggerSplitter.Dock = System.Windows.Forms.DockStyle.Right;
            this.loggerSplitter.Location = new System.Drawing.Point(1123, 29);
            this.loggerSplitter.Name = "loggerSplitter";
            this.loggerSplitter.Size = new System.Drawing.Size(3, 547);
            this.loggerSplitter.TabIndex = 6;
            this.loggerSplitter.TabStop = false;
            // 
            // appNotifyIcon
            // 
            this.appNotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.appNotifyIcon.ContextMenuStrip = this.trayContextMenuStrip;
            this.appNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("appNotifyIcon.Icon")));
            this.appNotifyIcon.Text = "appNotifyIcon";
            this.appNotifyIcon.Visible = true;
            this.appNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.appNotifyIcon_MouseDoubleClick);
            // 
            // trayContextMenuStrip
            // 
            this.trayContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreTrayMenuItem,
            this.toolStripSeparator5,
            this.settingsTrayMenuItem,
            this.aboutTrayMenuItem,
            this.toolStripSeparator6,
            this.exitTrayMenuItem});
            this.trayContextMenuStrip.Name = "trayContextMenuStrip";
            this.trayContextMenuStrip.Size = new System.Drawing.Size(190, 104);
            // 
            // restoreTrayMenuItem
            // 
            this.restoreTrayMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.restoreTrayMenuItem.Name = "restoreTrayMenuItem";
            this.restoreTrayMenuItem.Size = new System.Drawing.Size(189, 22);
            this.restoreTrayMenuItem.Text = "Restore";
            this.restoreTrayMenuItem.Click += new System.EventHandler(this.restoreTrayMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(186, 6);
            // 
            // settingsTrayMenuItem
            // 
            this.settingsTrayMenuItem.Name = "settingsTrayMenuItem";
            this.settingsTrayMenuItem.Size = new System.Drawing.Size(189, 22);
            this.settingsTrayMenuItem.Text = "Settings...";
            this.settingsTrayMenuItem.Click += new System.EventHandler(this.settingsTrayMenuItem_Click);
            // 
            // aboutTrayMenuItem
            // 
            this.aboutTrayMenuItem.Name = "aboutTrayMenuItem";
            this.aboutTrayMenuItem.Size = new System.Drawing.Size(189, 22);
            this.aboutTrayMenuItem.Text = "About Log2Window...";
            this.aboutTrayMenuItem.Click += new System.EventHandler(this.aboutTrayMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(186, 6);
            // 
            // exitTrayMenuItem
            // 
            this.exitTrayMenuItem.Name = "exitTrayMenuItem";
            this.exitTrayMenuItem.Size = new System.Drawing.Size(189, 22);
            this.exitTrayMenuItem.Text = "Exit";
            this.exitTrayMenuItem.Click += new System.EventHandler(this.exitTrayMenuItem_Click);
            // 
            // logDetailPanel
            // 
            this.logDetailPanel.Controls.Add(this.tabControlDetail);
            this.logDetailPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logDetailPanel.Location = new System.Drawing.Point(0, 382);
            this.logDetailPanel.Name = "logDetailPanel";
            this.logDetailPanel.Size = new System.Drawing.Size(1123, 194);
            this.logDetailPanel.TabIndex = 7;
            // 
            // tabControlDetail
            // 
            this.tabControlDetail.Controls.Add(this.tabMessage);
            this.tabControlDetail.Controls.Add(this.tabExceptions);
            this.tabControlDetail.Controls.Add(this.tabSource);
            this.tabControlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDetail.Location = new System.Drawing.Point(0, 0);
            this.tabControlDetail.Name = "tabControlDetail";
            this.tabControlDetail.SelectedIndex = 0;
            this.tabControlDetail.Size = new System.Drawing.Size(1123, 194);
            this.tabControlDetail.TabIndex = 2;
            // 
            // tabMessage
            // 
            this.tabMessage.Controls.Add(this.logDetailInnerPanel);
            this.tabMessage.Location = new System.Drawing.Point(4, 22);
            this.tabMessage.Name = "tabMessage";
            this.tabMessage.Padding = new System.Windows.Forms.Padding(3);
            this.tabMessage.Size = new System.Drawing.Size(1115, 168);
            this.tabMessage.TabIndex = 0;
            this.tabMessage.Text = "Message Details";
            this.tabMessage.UseVisualStyleBackColor = true;
            // 
            // logDetailInnerPanel
            // 
            this.logDetailInnerPanel.Controls.Add(this.tbMessage);
            this.logDetailInnerPanel.Controls.Add(this.logDetailTextBox);
            this.logDetailInnerPanel.Controls.Add(this.logDetailToolStrip);
            this.logDetailInnerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logDetailInnerPanel.Location = new System.Drawing.Point(3, 3);
            this.logDetailInnerPanel.Name = "logDetailInnerPanel";
            this.logDetailInnerPanel.Size = new System.Drawing.Size(1109, 162);
            this.logDetailInnerPanel.TabIndex = 1;
            // 
            // tbMessage
            // 
            this.tbMessage.BackColor = System.Drawing.SystemColors.Window;
            this.tbMessage.DetectUrls = false;
            this.tbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMessage.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMessage.Location = new System.Drawing.Point(300, 25);
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.ReadOnly = true;
            this.tbMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.tbMessage.Size = new System.Drawing.Size(809, 137);
            this.tbMessage.TabIndex = 2;
            this.tbMessage.Text = "";
            // 
            // logDetailTextBox
            // 
            this.logDetailTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.logDetailTextBox.DetectUrls = false;
            this.logDetailTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.logDetailTextBox.Location = new System.Drawing.Point(0, 25);
            this.logDetailTextBox.Name = "logDetailTextBox";
            this.logDetailTextBox.ReadOnly = true;
            this.logDetailTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.logDetailTextBox.Size = new System.Drawing.Size(300, 137);
            this.logDetailTextBox.TabIndex = 0;
            this.logDetailTextBox.Text = "";
            // 
            // logDetailToolStrip
            // 
            this.logDetailToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.toolStripSeparator11,
            this.zoomOutLogDetailsBtn,
            this.zoomInLogDetailsBtn,
            this.toolStripSeparator7,
            this.copyLogDetailBtn});
            this.logDetailToolStrip.Location = new System.Drawing.Point(0, 0);
            this.logDetailToolStrip.Name = "logDetailToolStrip";
            this.logDetailToolStrip.Size = new System.Drawing.Size(1109, 25);
            this.logDetailToolStrip.TabIndex = 1;
            this.logDetailToolStrip.Text = "toolStrip2";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(96, 22);
            this.toolStripLabel2.Text = "Message Details";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // zoomOutLogDetailsBtn
            // 
            this.zoomOutLogDetailsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutLogDetailsBtn.Image = global::Log2Window.Properties.Resources.zoomout16;
            this.zoomOutLogDetailsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutLogDetailsBtn.Name = "zoomOutLogDetailsBtn";
            this.zoomOutLogDetailsBtn.Size = new System.Drawing.Size(23, 22);
            this.zoomOutLogDetailsBtn.ToolTipText = "Zoom Out Log Details Font";
            this.zoomOutLogDetailsBtn.Click += new System.EventHandler(this.zoomOutLogDetailsBtn_Click);
            // 
            // zoomInLogDetailsBtn
            // 
            this.zoomInLogDetailsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInLogDetailsBtn.Image = global::Log2Window.Properties.Resources.zoomin16;
            this.zoomInLogDetailsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInLogDetailsBtn.Name = "zoomInLogDetailsBtn";
            this.zoomInLogDetailsBtn.Size = new System.Drawing.Size(23, 22);
            this.zoomInLogDetailsBtn.ToolTipText = "Zoom In Log Details Font";
            this.zoomInLogDetailsBtn.Click += new System.EventHandler(this.zoomInLogDetailsBtn_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // copyLogDetailBtn
            // 
            this.copyLogDetailBtn.Image = global::Log2Window.Properties.Resources.documentsorcopy16;
            this.copyLogDetailBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyLogDetailBtn.Name = "copyLogDetailBtn";
            this.copyLogDetailBtn.Size = new System.Drawing.Size(55, 22);
            this.copyLogDetailBtn.Text = "Copy";
            this.copyLogDetailBtn.Click += new System.EventHandler(this.copyLogDetailBtn_Click);
            // 
            // tabExceptions
            // 
            this.tabExceptions.Controls.Add(this.tbExceptions);
            this.tabExceptions.Location = new System.Drawing.Point(4, 22);
            this.tabExceptions.Name = "tabExceptions";
            this.tabExceptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabExceptions.Size = new System.Drawing.Size(1115, 168);
            this.tabExceptions.TabIndex = 2;
            this.tabExceptions.Text = "Exceptions";
            this.tabExceptions.UseVisualStyleBackColor = true;
            // 
            // tbExceptions
            // 
            this.tbExceptions.BackColor = System.Drawing.SystemColors.Window;
            this.tbExceptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbExceptions.Location = new System.Drawing.Point(3, 3);
            this.tbExceptions.Name = "tbExceptions";
            this.tbExceptions.ReadOnly = true;
            this.tbExceptions.Size = new System.Drawing.Size(1109, 162);
            this.tbExceptions.TabIndex = 0;
            this.tbExceptions.Text = "";
            this.tbExceptions.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.TbExceptionsLinkClicked);
            // 
            // tabSource
            // 
            this.tabSource.Controls.Add(this.panel1);
            this.tabSource.Location = new System.Drawing.Point(4, 22);
            this.tabSource.Name = "tabSource";
            this.tabSource.Padding = new System.Windows.Forms.Padding(3);
            this.tabSource.Size = new System.Drawing.Size(1115, 168);
            this.tabSource.TabIndex = 1;
            this.tabSource.Text = "Source Code";
            this.tabSource.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textEditorSourceCode);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1109, 162);
            this.panel1.TabIndex = 1;
            // 
            // textEditorSourceCode
            // 
            this.textEditorSourceCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditorSourceCode.IsReadOnly = false;
            this.textEditorSourceCode.LineViewerStyle = ICSharpCode.TextEditor.Document.LineViewerStyle.FullRow;
            this.textEditorSourceCode.Location = new System.Drawing.Point(0, 25);
            this.textEditorSourceCode.Name = "textEditorSourceCode";
            this.textEditorSourceCode.Size = new System.Drawing.Size(1109, 137);
            this.textEditorSourceCode.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel5,
            this.lbFileName,
            this.btnOpenFileInVS});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1109, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(29, 22);
            this.toolStripLabel5.Text = "File:";
            // 
            // lbFileName
            // 
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(0, 22);
            // 
            // btnOpenFileInVS
            // 
            this.btnOpenFileInVS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenFileInVS.Image = global::Log2Window.Properties.Resources.saveas16;
            this.btnOpenFileInVS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenFileInVS.Name = "btnOpenFileInVS";
            this.btnOpenFileInVS.Size = new System.Drawing.Size(23, 22);
            this.btnOpenFileInVS.Text = "Export Logs...";
            this.btnOpenFileInVS.Click += new System.EventHandler(this.btnOpenFileInVS_Click);
            // 
            // logDetailSplitter
            // 
            this.logDetailSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logDetailSplitter.Location = new System.Drawing.Point(0, 379);
            this.logDetailSplitter.Name = "logDetailSplitter";
            this.logDetailSplitter.Size = new System.Drawing.Size(1123, 3);
            this.logDetailSplitter.TabIndex = 8;
            this.logDetailSplitter.TabStop = false;
            // 
            // loggerTreeContextMenuStrip
            // 
            this.loggerTreeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteLoggerTreeMenuItem,
            this.toolStripSeparator16,
            this.deleteAllLoggerTreeMenuItem});
            this.loggerTreeContextMenuStrip.Name = "loggerTreeContextMenuStrip";
            this.loggerTreeContextMenuStrip.Size = new System.Drawing.Size(164, 54);
            // 
            // deleteLoggerTreeMenuItem
            // 
            this.deleteLoggerTreeMenuItem.Name = "deleteLoggerTreeMenuItem";
            this.deleteLoggerTreeMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteLoggerTreeMenuItem.Text = "Clear Logger";
            this.deleteLoggerTreeMenuItem.Click += new System.EventHandler(this.deleteLoggerTreeMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(160, 6);
            // 
            // deleteAllLoggerTreeMenuItem
            // 
            this.deleteAllLoggerTreeMenuItem.Name = "deleteAllLoggerTreeMenuItem";
            this.deleteAllLoggerTreeMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteAllLoggerTreeMenuItem.Text = "Clear All Loggers";
            this.deleteAllLoggerTreeMenuItem.Click += new System.EventHandler(this.deleteAllLoggerTreeMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "All files|*.*";
            this.openFileDialog1.Title = "Open Log File";
            // 
            // logListView
            // 
            this.logListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader5});
            this.logListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logListView.FullRowSelect = true;
            this.logListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.logListView.HideSelection = false;
            this.logListView.Location = new System.Drawing.Point(0, 29);
            this.logListView.Name = "logListView";
            this.logListView.ShowItemToolTips = true;
            this.logListView.Size = new System.Drawing.Size(1123, 350);
            this.logListView.TabIndex = 0;
            this.logListView.UseCompatibleStateImageBehavior = false;
            this.logListView.View = System.Windows.Forms.View.Details;
            this.logListView.SelectedIndexChanged += new System.EventHandler(this.logListView_SelectedIndexChanged);
            this.logListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.logListView_KeyDown);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Nr";
            this.columnHeader6.Width = 40;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Level";
            this.columnHeader2.Width = 48;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Logger";
            this.columnHeader3.Width = 92;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Thread";
            this.columnHeader4.Width = 52;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "CallSiteClass";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "CallSiteMethod";
            this.columnHeader8.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Message";
            this.columnHeader5.Width = 540;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1363, 576);
            this.Controls.Add(this.logListView);
            this.Controls.Add(this.logDetailSplitter);
            this.Controls.Add(this.logDetailPanel);
            this.Controls.Add(this.loggerSplitter);
            this.Controls.Add(this.loggerPanel);
            this.Controls.Add(this.mainToolStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Log2Window";
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.loggerPanel.ResumeLayout(false);
            this.loggerInnerPanel.ResumeLayout(false);
            this.loggerInnerPanel.PerformLayout();
            this.loggersToolStrip.ResumeLayout(false);
            this.loggersToolStrip.PerformLayout();
            this.trayContextMenuStrip.ResumeLayout(false);
            this.logDetailPanel.ResumeLayout(false);
            this.tabControlDetail.ResumeLayout(false);
            this.tabMessage.ResumeLayout(false);
            this.logDetailInnerPanel.ResumeLayout(false);
            this.logDetailInnerPanel.PerformLayout();
            this.logDetailToolStrip.ResumeLayout(false);
            this.logDetailToolStrip.PerformLayout();
            this.tabExceptions.ResumeLayout(false);
            this.tabSource.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.loggerTreeContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }  




        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton clearBtn;
        private System.Windows.Forms.ColumnHeader timeColumnHeader;
        private System.Windows.Forms.ColumnHeader levelColumnHeader;
        private System.Windows.Forms.ColumnHeader loggerColumnHeader;
        private System.Windows.Forms.ColumnHeader threadColumnHeader;
        private System.Windows.Forms.ColumnHeader msgColumnHeader;
        private Log2Window.UI.FlickerFreeListView logListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private TreeViewWithoutDoubleClick loggerTreeView;
        private System.Windows.Forms.Panel loggerPanel;
        private System.Windows.Forms.Panel loggerInnerPanel;
        private System.Windows.Forms.Splitter loggerSplitter;
        private System.Windows.Forms.ToolStrip loggersToolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton clearLoggersBtn;
		private System.Windows.Forms.NotifyIcon appNotifyIcon;
        private System.Windows.Forms.ToolStripButton aboutBtn;
        private System.Windows.Forms.ToolStripButton settingsBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ContextMenuStrip trayContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem restoreTrayMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutTrayMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsTrayMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem exitTrayMenuItem;
        private System.Windows.Forms.Panel logDetailPanel;
        private System.Windows.Forms.Splitter logDetailSplitter;
        private System.Windows.Forms.Panel logDetailInnerPanel;
        private System.Windows.Forms.RichTextBox logDetailTextBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton copyLogDetailBtn;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel lblSearch;
        private MyToolStripTextBox searchTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton zoomOutLogListBtn;
        private System.Windows.Forms.ToolStripButton zoomInLogListBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton zoomOutLogDetailsBtn;
        private System.Windows.Forms.ToolStripButton zoomInLogDetailsBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton pinOnTopBtn;
        private System.Windows.Forms.ToolStripButton receiversBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripButton pauseRefreshNewMessagesBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton goToFirstLogBtn;
        private System.Windows.Forms.ToolStripButton goToLastLogBtn;
        private System.Windows.Forms.ContextMenuStrip loggerTreeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteLoggerTreeMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem deleteAllLoggerTreeMenuItem;
        private System.Windows.Forms.TabControl tabControlDetail;
        private System.Windows.Forms.TabPage tabMessage;
        private System.Windows.Forms.TabPage tabSource;
        private ICSharpCode.TextEditor.TextEditorControl textEditorSourceCode;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private ToolStripLabel lbFileName;
        private ToolStrip logDetailToolStrip;
        private ToolStripButton btnOpenFileInVS;
        private TabPage tabExceptions;
        private RichTextBoxLinks.RichTextBoxEx tbExceptions;
        private OpenFileDialog openFileDialog1;
        private ToolStripButton dactivateSourcesBtn;
        private ToolStripButton collapseAllBtn;
        private ToolStripButton keepHighlightBtn;
        private RichTextBox tbMessage;
        private ToolStripButton btnTrace;
        private ToolStripButton btnDebug;
        private ToolStripButton btnInfo;
        private ToolStripButton btnWarn;
        private ToolStripButton btnError;
        private ToolStripButton btnFatal;
        private ToolStripDropDownButton ddbEventLog;
        private ToolStripMenuItem miLoadEventLog;
        private ToolStripMenuItem miClearEventLog;
        private ToolStripDropDownButton ddbExportBtn;
        private ToolStripMenuItem miExportLog4jXmlFile;
        private ToolStripMenuItem miExportExcelCsvFile;
        private ToolStripDropDownButton ddbOpenLogFileBtn;
        private ToolStripMenuItem miOpenLog4jXmlFile;
        private ToolStripMenuItem miOpenPatternLayoutFile;
        private MyToolStripTextBox searchThreadBox;
    }
}

