using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Log2Window.Log
{
    public class LogManager
    {
        private static LogManager _instance;

        //_allLogMessageItems and _dataSource must use this locker.        
        public readonly object dataLocker = new object();
        //_allLogMessageItems may be reassigned, so cannot use as a locker.
        public MyList<LogMessageItem> _allLogMessageItems = new MyList<LogMessageItem>();
        public MyList<LogMessageItem> _dataSource = new MyList<LogMessageItem>();

        private LoggerItem _rootLoggerItem;
        private Dictionary<string, LoggerItem> _fullPathLoggers;
        public ListView _logListView;

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
                _allLogMessageItems.Enqueue(item);
                if (item.Enabled && !LogManager.Instance.PauseRefreshNewMessages)
                {
                    _dataSource.Enqueue(item);
                }

                var maxCount = Settings.UserSettings.Instance.MessageCycleCount;
                if (maxCount > 0)
                {
                    while (_allLogMessageItems.Count > maxCount)
                    {
                        var tobeRemoveItem = _allLogMessageItems[0];
                        _allLogMessageItems.Dequeue();
                        if (_dataSource.Count > 0
                            && !LogManager.Instance.PauseRefreshNewMessages
                            )
                        { 
                            //remove all messages which ArrivedId <= tobeRemoveItem's ArrivedId.
                            while (_dataSource.Peek().Message.ArrivedId <= tobeRemoveItem.Message.ArrivedId)
                            {
                                _dataSource.Dequeue();
                            }
                        } 
                    }
                }
            }

        }


        public void SearchText(string str)
        {
            _rootLoggerItem.SearchText(str);
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
    }
}
