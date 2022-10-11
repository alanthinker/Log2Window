using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;


namespace Log2Window.Log
{
    public class LogManager
    {
        private static LogManager _instance;

        //_allLogMessageItems and _dataSource must use this locker.        
        public readonly object dataLocker = new object();
        //_allLogMessageItems may be reassigned, so cannot use as a locker.
        public MyCategoryList<LogMessageItem, LogLevel> _allLogMessageItems = new MyCategoryList<LogMessageItem, LogLevel>(new List<LogLevel> { LogLevel.Fatal, LogLevel.Error, LogLevel.Warn, LogLevel.Info, LogLevel.Debug, LogLevel.Trace });
        public MyList<LogMessageItem> _dataSource = new MyList<LogMessageItem>();

        private LoggerItem _rootLoggerItem;
        private Dictionary<string, LoggerItem> _fullPathLoggers;
        public ListView _logListView;
        internal ulong manulSelectedArrivedId;

        public bool IsDelay { get; set; }

        private LogManager()
        {

        }

        internal static LogManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LogManager();
                return _instance;
            }
        }

        internal LoggerItem RootLoggerItem
        {
            get { return _rootLoggerItem; }
            set { _rootLoggerItem = value; }
        }

        public bool PauseRefreshNewMessages { get; set; }

        public void Initialize(ILoggerView loggerView, ListView logListView)
        {
            _logListView = logListView;
            // Root Logger
            _rootLoggerItem = LoggerItem.CreateRootLoggerItem("Root", loggerView, logListView);

            // Quick Access Logger Collection
            _fullPathLoggers = new Dictionary<string, LoggerItem>();
        }

        public void ClearAll()
        {
            ClearLogMessages();
            RootLoggerItem.ClearAll();
            _fullPathLoggers.Clear();
        }

        public void ClearLogMessages()
        {
            lock (dataLocker)
            {
                _allLogMessageItems.Clear();
                _dataSource.Clear();
                MainForm.Instance.ReBindListViewFromAllLogMessageItems();

                GC.Collect();
            }
        }

        public void DeactivateLogger()
        {
            RootLoggerItem.Enabled = false;
        }

        public void ProcessLogMessage(LogMessage logMsg)
        {
            // Check 1st in the global LoggerPath/Logger dictionary
            LoggerItem logger;
            logMsg.CheckNull();

            // Search by thread need compare ThreadName. So need trim.
            logMsg.ThreadName = logMsg.ThreadName?.Trim();

            if (!_fullPathLoggers.TryGetValue(logMsg.LoggerName, out logger))
            {
                // Not found, create one
                logger = RootLoggerItem.GetOrCreateLogger(logMsg.LoggerName);
            }
            if (logger == null)
                throw new Exception("No Logger for this Log Message.");

            var item = logger.AddLogMessage(logMsg);

            lock (LogManager.Instance.dataLocker)
            {
                _allLogMessageItems.Enqueue(item, item.Message.Level.Level);
                if (item.Enabled && !LogManager.Instance.PauseRefreshNewMessages)
                {
                    _dataSource.Enqueue(item);
                }

                // 只在 TryEnsureVisibleForSuitableItems 函数中调用 DequeueMoreThanMaxCount, 防止新增消息的时候, 影响listview的index指向的实际行.
                //DequeueMoreThanMaxCount(); 
            }

        }

        public long DequeueMoreThanMaxCount()
        {
            long removedCount=0;
            var maxCount = Settings.UserSettings.Instance.MessageCycleCountForEachLevel;
            if (maxCount > 0)
            {
                removedCount = _allLogMessageItems.DequeueSmart(maxCount);
                if (removedCount > 0)
                {
                    if (!LogManager.Instance.PauseRefreshNewMessages
                           )
                    {
                        MainForm.Instance.ReBindListViewFromAllLogMessageItems(false);
                    }
                }
            }
            GC.Collect();
            return removedCount;
        }

        public void allLogMessageItems_to_dataSource()
        {
            this._dataSource.Clear();
            var temp = new MyCategoryList<LogMessageItem, LogLevel>(new List<LogLevel> { LogLevel.Fatal, LogLevel.Error, LogLevel.Warn, LogLevel.Info, LogLevel.Debug, LogLevel.Trace });
            foreach (var item in this._allLogMessageItems)
            {
                item.Enabled = item.Parent.IsItemToBeEnabled(item);
                if (item.Enabled)
                {
                    temp.Enqueue(item, item.Message.Level.Level);
                }
            }


            var listList = temp.ToListList();
            var t0 = DateTime.Now;
            this._dataSource = new NListsMerger<LogMessageItem>().MergeNLists(listList);
            var t1 = DateTime.Now;
            Trace.WriteLine("ts=" + (t1 - t0));
        }

        public void SearchText(string str)
        {
            _rootLoggerItem.SearchText(str);
        }


        public void SearchByThread(string str)
        {
            _rootLoggerItem.SearchByThread(str);
        }       


        public void UpdateLogLevel()
        {
            if (RootLoggerItem == null)
                return;

            RootLoggerItem.UpdateLogLevel();
        }

        public void SetRootLoggerName(string name)
        {
            RootLoggerItem.Name = name;
        }

        internal bool inSetSelectedIndicesByCode;
    }
}
