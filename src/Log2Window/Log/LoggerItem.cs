using Log2Window.Settings;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;

namespace Log2Window.Log
{
    /// <summary>
    /// Describes a Logger.
    /// </summary>
    public class LoggerItem
    {
        private const char LoggerSeparator = '.';


        /// <summary>
        /// Parent Logger Item. Null for the Root.
        /// </summary>
        public LoggerItem Parent;
        /// <summary>
        /// Collection of child Logger Items, identified by their full path.
        /// </summary>
        public Dictionary<string, LoggerItem> Loggers = new Dictionary<string, LoggerItem>();


        /// <summary>
        /// The associated Tree Node.
        /// </summary>
        public ILoggerView LoggerView;

        /// <summary>
        /// A reference to the Log ListView associated to this Logger.
        /// </summary>
        private ListView _logListView;

        /// <summary>
        /// Short Name of this Logger (used as the node name).
        /// </summary>
        private string _name = String.Empty;
        /// <summary>
        /// Full Name (or "Path") of this Logger.
        /// </summary>
        public string FullName = String.Empty;
        /// <summary>
        /// When set the Logger and its Messages are displayed.
        /// </summary>
        private bool _enabled = true;
        public bool _messagesDeleted = false;

        private string _searchedText;
        private bool _hasSearchedText;


        private LoggerItem()
        {
        }

        public static LoggerItem CreateRootLoggerItem(string name, ILoggerView loggerView, ListView logListView)
        {
            LoggerItem logger = new LoggerItem();
            logger.Name = name;
            logger._logListView = logListView;

            // Tree Node
            logger.LoggerView = loggerView.AddNew(name, logger);

            return logger;
        }

        private static LoggerItem CreateLoggerItem(string name, string fullName, LoggerItem parent)
        {
            if (parent == null)
                throw new ArgumentNullException();

            // Creating the logger item.
            LoggerItem logger = new LoggerItem();
            logger.Name = name;
            logger.FullName = fullName;
            logger.Parent = parent;


            logger._logListView = logger.Parent._logListView;

            // Adding the logger as a child of the parent logger.
            logger.Parent.Loggers.Add(name, logger);

            // Creating a child logger view and saving it in the new logger.
            logger.LoggerView = parent.LoggerView.AddNew(name, logger);

            if (UserSettings.Instance.RecursivlyEnableLoggers)
            {
                logger._enabled = parent.Enabled;
                logger.LoggerView.Enabled = parent.LoggerView.Enabled;
            }


            return logger;
        }



        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                if (LoggerView != null)
                    LoggerView.Text = _name;
            }
        }

        internal bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value)
                    return;

                _enabled = value;
                LoggerView.Enabled = value;

                //Utils.log.Debug(this.Name + " " + value);

                // Now enable all child loggers if the settings are set to 
                // recursivly enable/disable chid loggers.
                if (UserSettings.Instance.RecursivlyEnableLoggers)
                {
                    foreach (LoggerItem child in Loggers.Values)
                    {
                        child.Enabled = value;
                    }
                }
            }
        }

        //private void SortDataItems()
        //{
        //    var tempItems = _logListView.Items.Cast<ListViewItem>().ToArray();
        //    tempItems = tempItems.OrderBy(x => (x.Tag as LogMessageItem).ArrivedId).ToArray();
        //    _logListView.Items.Clear();
        //    _logListView.Items.AddRange(tempItems);
        //}



        public void Remove()
        {
            if (Parent == null)
            {
                // If root, clear all
                ClearAll();
                return;
            }

            ClearLogMessages();

            Parent.Loggers.Remove(Name);
            Parent.LoggerView.Remove(Name);
        }

        public void ClearAll()
        {
            ClearAllLogMessages();

            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
                kvp.Value.ClearAll();

            if (LoggerView != null)
                LoggerView.Clear();
            Loggers.Clear();
        }

        public void ClearAllLogMessages()
        {

            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
                kvp.Value.ClearLogMessages();
        }

        public void ClearLogMessages()
        {
            _messagesDeleted = true;

            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
                kvp.Value.ClearLogMessages();
        }


        internal void UpdateLogLevel()
        {
            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
            {
                kvp.Value.UpdateLogLevel();
            }
        }

        private void EnableLogMessage(LogMessageItem item, bool enable)
        {
            if ((item.Enabled && enable) || (!item.Enabled && !enable))
                return;

            if (enable)
                EnableLogMessage(item);
            else
                DisableLogMessage(item);
        }

        private void EnableLogMessage(LogMessageItem item)
        {
            if (item.Enabled)
                return;

            item.Enabled = Enabled;
        }

        private void DisableLogMessage(LogMessageItem item)
        {
            if (!item.Enabled)
                return;

            // Mark the item as disabled
            item.Enabled = false;
        }



        internal LoggerItem GetOrCreateLogger(string loggerPath)
        {
            if (String.IsNullOrEmpty(loggerPath))
                return null;

            // Extract Logger Name
            string currentLoggerName = loggerPath;
            string remainingLoggerPath = String.Empty;
            int pos = loggerPath.IndexOf(LoggerSeparator);
            if (pos > 0)
            {
                currentLoggerName = loggerPath.Substring(0, pos);
                remainingLoggerPath = loggerPath.Substring(pos + 1);
            }

            // Check if the Logger is in the Child Collection
            LoggerItem logger;
            if (!Loggers.TryGetValue(currentLoggerName, out logger))
            {
                // Not found here, needs to be created
                string childLoggerPath =
                    (String.IsNullOrEmpty(FullName) ? "" : FullName + LoggerSeparator) + currentLoggerName;
                logger = CreateLoggerItem(currentLoggerName, childLoggerPath, this);
            }

            // Continue?
            if (!String.IsNullOrEmpty(remainingLoggerPath))
                logger = logger.GetOrCreateLogger(remainingLoggerPath);

            return logger;
        }

        internal LogMessageItem AddLogMessage(LogMessage logMsg)
        {
            LogMessageItem item = new LogMessageItem(this, logMsg);
            //item.ArrivedId = IdCreator.GetNextId();
            item.Enabled = Enabled;

            //int index = 0;

            // 不再使用 Previous

            //if (_logListView.Items.Count > 0)
            //{
            //    for (index = _logListView.Items.Count; index > 0; --index)
            //    {
            //        item.Previous = _logListView.Items[index - 1].Tag as LogMessageItem;
            //        if (item.Previous.Message.TimeStamp.Ticks <= item.Message.TimeStamp.Ticks)
            //            break;
            //    }
            //}  

            // Message
            if (Enabled)
            {
                // Add it to the main list
                //_logListView.Items.Insert(index, item.Item); 


                if (IsItemToBeEnabled(item))
                {
                    item.Enabled = true;
                }
                else
                {
                    item.Enabled = false;
                }

                TryEnsureVisibleLastItem(_logListView);
            }

            return item;
        }

        public static DateTime lastEnsureVisibleTime = DateTime.MinValue;
        public static ulong lastEnsureVisibleArrivedId = 0;
        public static TimeSpan EnsureVisiblePeroid = TimeSpan.FromSeconds(0.1);

        public static void TryEnsureVisibleLastItem(ListView logListView)
        {
            if (LogManager.Instance.PauseRefreshNewMessages)
                return;

            if (lastEnsureVisibleTime > DateTime.Now) //PC time may changed by user.
                lastEnsureVisibleTime = DateTime.Now - EnsureVisiblePeroid - EnsureVisiblePeroid; //let EnsureVisible trigger at once.

            if (DateTime.Now - lastEnsureVisibleTime > EnsureVisiblePeroid)
            {
                logListView.Invoke(new Action(delegate ()
                {
                    lock (LogManager.Instance.dataLocker)
                    {
                        logListView.VirtualListSize = LogManager.Instance._dataSource.Count;
                        if (LogManager.Instance._dataSource.Count > 0
                        )
                        {
                            var index = LogManager.Instance._dataSource.Count - 1;
                            var thisArrivedId = LogManager.Instance._dataSource[index].Message.ArrivedId;

                            if (thisArrivedId != lastEnsureVisibleArrivedId)
                            {
                                var speed = (double)(thisArrivedId - lastEnsureVisibleArrivedId) / Math.Max(0.01, (DateTime.Now - lastEnsureVisibleTime).Seconds);
                                //Utils.log.Debug("speed:" + speed);
                                //收到的消息速度越快, 下次间隔就越大, 可以改进性能. 
                                //  如果1秒收到1000个消息, 那1秒后再收.
                                //  如果1秒收到100个消息, 那0.1秒后再收.
                                EnsureVisiblePeroid = TimeSpan.FromSeconds(Math.Max(0.1, Math.Min(1, speed / 1000)));
                                Utils.log.Debug("EnsureVisiblePeroid:" + EnsureVisiblePeroid + " speed:" + speed + " index:" + index);
                                LoggerItem.lastEnsureVisibleTime = DateTime.Now;
                                lastEnsureVisibleArrivedId = thisArrivedId;

                                logListView.EnsureVisible(index);
                                logListView.SelectedIndices.Clear();
                                //logListView.SelectedIndices.Add(index);

                                MainForm.Instance.RefreshTitle();

                                // If use MessageCycle, VirtualListSize and EnsureVisible index may not changed.
                                // So force Refresh it.
                                if (UserSettings.Instance.MessageCycleCount > 0)
                                    logListView.Refresh();
                            }
                        }
                    }
                }));
            }
        }


        internal void SearchText(string str)
        {
            _logListView.BeginUpdate();

            DoSearch(str);

            _logListView.EndUpdate();
        }

        private void DoSearch(string str)
        {
            _hasSearchedText = !String.IsNullOrEmpty(str);
            _searchedText = str;

            // Iterate call
            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
            {
                kvp.Value.DoSearch(_searchedText);
            }
        }

        internal bool IsItemToBeEnabled(LogMessageItem item)
        {
            return (this.Enabled && item.IsLevelInRange() && (!_hasSearchedText || item.HasSearchedText(_searchedText)));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
