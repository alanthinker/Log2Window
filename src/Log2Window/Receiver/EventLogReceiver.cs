using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Log2Window.Log;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Log2Window.Receiver
{
    [Serializable]
    [DisplayName("Windows Event Log")]
    public class EventLogReceiver : BaseReceiver
    {
        [NonSerialized]
        private EventLog[] _eventLogs;

        private string _logName;
        private string _machineName = ".";
        private string _source;
        private bool _appendHostNameToLogger = true;
        private bool _showFromBeginning = true;


        [Category("Configuration")]
        [DisplayName("Event Log Name")]
        [Description("The name of the log on the specified computer. Such as 'Application', 'System', 'Security'. Leave empty to show all logs.(Need administrator.)")]
        public string LogName
        {
            get { return _logName; }
            set { _logName = value; }
        }

        [Category("Configuration")]
        [DisplayName("Machine Name")]
        [Description("The computer on which the log exists.")]
        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }

        [Category("Configuration")]
        [DisplayName("Event Log Source")]
        [Description("The source of event log entries. Such as 'Windows Error Reporting'. Leave empty to show all source in the log.")]
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        [Category("Behavior")]
        [DisplayName("Append Machine Name to Logger")]
        [Description("Append the remote Machine Name to the Logger Name.")]
        public bool AppendHostNameToLogger
        {
            get { return _appendHostNameToLogger; }
            set { _appendHostNameToLogger = value; }
        }

        [Category("Configuration")]
        [DisplayName("Show from Beginning")]
        [Description("Show all log messages from the beginning (not just newly added log messages.)")]
        [DefaultValue(true)]
        public bool ShowFromBeginning
        {
            get { return _showFromBeginning; }
            set
            {
                _showFromBeginning = value;
            }
        }

        //[NonSerialized]
        //private string _baseLoggerName;


        #region Overrides of BaseReceiver

        [Browsable(false)]
        public override string TextEncoding
        {
            get;
            set;
        }

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return "Use Log2Window to display the Windows Event Logs." + Environment.NewLine +
                       "Note that the Thread column is used to display the Instance ID (Event ID).";
            }
        }

        public override void Initialize()
        {
            if (String.IsNullOrEmpty(MachineName))
                MachineName = ".";

            if (String.IsNullOrEmpty(LogName))
            {
                _eventLogs = EventLog.GetEventLogs();
            }
            else
            {
                _eventLogs = new EventLog[1];
                if (string.IsNullOrEmpty(Source))
                {
                    _eventLogs[0] = new EventLog(LogName, MachineName);
                }
                else
                {
                    _eventLogs[0] = new EventLog(LogName, MachineName, Source);
                }
            }

            foreach (var eventLog in _eventLogs)
            {
                try
                {
                    //sender is not EventLog type, use lamda expresstion to get outer eventLog variable.
                    eventLog.EntryWritten += delegate (object sender, EntryWrittenEventArgs entryWrittenEventArgs)
                    {
                        var entry = entryWrittenEventArgs.Entry;
                        ParseEventLogEntry(eventLog, entry);
                    };

                    eventLog.EnableRaisingEvents = true;
                }
                catch (Exception ex)
                {
                    Utils.log.Error(ex.Message, ex);
                    System.Threading.ThreadPool.QueueUserWorkItem(delegate (object ob)
                       {
                           MessageBox.Show(ex.Message, "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                       });
                }
            }

            //_baseLoggerName = AppendHostNameToLogger && !String.IsNullOrEmpty(MachineName) && (MachineName != ".")
            //                        ? String.Format("[Host: {0}].{1}", MachineName, LogName)
            //                        : LogName; 
        }

        public override void Attach(ILogMessageNotifiable notifiable)
        {
            base.Attach(notifiable);


            List<Tuple<EventLog, EventLogEntry>> data = new List<Tuple<EventLog, EventLogEntry>>();


            if (ShowFromBeginning)
            {
                foreach (var eventLog in _eventLogs)
                {
                    try
                    {
                        foreach (EventLogEntry entry in eventLog.Entries)
                        {
                            if (!string.IsNullOrEmpty(this.Source))
                            {
                                if (entry.Source != this.Source)
                                {
                                    continue;
                                }
                            }
                            data.Add(Tuple.Create(eventLog, entry));
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.log.Error(ex.Message, ex);
                    }

                }
            }

            data = data.OrderBy(x => x.Item2.TimeGenerated).ToList();

            foreach (var item in data)
            {
                ParseEventLogEntry(item.Item1, item.Item2);
            }
        }

        public override void Terminate()
        {
            foreach (var eventLog in _eventLogs)
            {
                eventLog.Dispose();
            }

            _eventLogs = null;
        }

        #endregion  

        private void ParseEventLogEntry(EventLog eventLog, EventLogEntry entry)
        {
            LogMessage logMsg = new LogMessage();
            var baseName = "EventLog." + eventLog.Log;
            if (AppendHostNameToLogger)
            {
                baseName = "EventLog_" + (this.MachineName == "." ? "local" : this.MachineName) + "." + eventLog.Log;
            }
            logMsg.RootLoggerName = baseName;
            logMsg.LoggerName = String.IsNullOrEmpty(entry.Source)
                                    ? baseName
                                    : String.Format("{0}.{1}", baseName, entry.Source);

            logMsg.Message = entry.Message;
            logMsg.TimeStamp = entry.TimeGenerated;
            logMsg.Level = LogUtils.GetLogLevelInfo(GetLogLevel(entry.EntryType));
            logMsg.ThreadName = entry.InstanceId.ToString();

            if (!String.IsNullOrEmpty(entry.Category))
                logMsg.Properties.Add("Category", entry.Category);
            if (!String.IsNullOrEmpty(entry.UserName))
                logMsg.Properties.Add("User Name", entry.UserName);

            if (Notifiable != null)
                Notifiable.Notify(logMsg);
        }

        private static LogLevel GetLogLevel(EventLogEntryType entryType)
        {
            switch (entryType)
            {
                case EventLogEntryType.Warning: return LogLevel.Warn;
                case EventLogEntryType.FailureAudit:
                case EventLogEntryType.Error: return LogLevel.Error;
                case EventLogEntryType.SuccessAudit:
                case EventLogEntryType.Information: return LogLevel.Info;
                default:
                    return LogLevel.None;
            }
        }
    }
}
