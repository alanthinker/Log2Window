using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using Log2Window.Log;
using System.Threading;

namespace Log2Window.Receiver
{
    [Serializable]
    [DisplayName("Log File (Log4j XML Formatted)")]
    public class Log4jFileReceiver : FileReceiver
    {
        static char[] log4jEndTag = "</log4j:event>".ToCharArray();

        public override void Initialize()
        {
            base.Initialize();

            sb = new StringBuilder();
        }

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return @"Configuration for nlog:
<target name='log4jfile' xsi:type='File' encoding='utf-8' fileName='${basedir}/log/log4jfile.log' layout='${log4jxmlevent}' />      

Configuration for log4net:
<appender name='fileLog4j' type='log4net.Appender.FileAppender'>
    <file value='log/fileLog4j.log' />
    <encoding value='utf-8'></encoding>
    <appendToFile value='true' /> 
    <layout type='log4net.Layout.XmlLayoutSchemaLog4j' />
</appender> 

OR

<appender name='rollingFileLog4j' type='log4net.Appender.RollingFileAppender,log4net' >
    <File value='log/rollingFileLog4j.log' />
    <Encoding value='utf-8' /> 
    <AppendToFile value='true' />
    <RollingStyle value='Composite' />
    <DatePattern value='.yyyy.MM.dd.'log'' />
    <MaximumFileSize value='20MB' />
    <maxSizeRollBackups value='10' />
    <!--file number increase after the file's size exceeds the MaximumFileSize.-->
    <CountDirection value='1' />
    <StaticLogFileName value='true' />
    <layout type='AlanThinker.MyLog4net.MyXmlLayoutSchemaLog4j' />
</appender>
".Replace("'", "\"").Replace("\n", Environment.NewLine);
            }
        }

        private bool IsEndWith(StringBuilder sb, char[] str)
        {
            if (sb.Length < str.Length)
                return false;

            for (int i = str.Length - 1, j = sb.Length - 1; i >= 0; i--, j--)
            {
                if (str[i] != sb[j])
                {
                    return false;
                }
            }

            return true;
        }

        //if log4net set ImmediateFlush=false, log message may only write only a part.
        //So need stock this part.
        [NonSerialized]
        StringBuilder sb = new StringBuilder();
        protected override void ProcessFileData()
        {
            int temp;
            while ((temp = fileReader.Read()) != -1)
            {
                // < ImmediateFlush value = "false" /> 有错, 尝试tcpreceiver的方法.
                sb.Append((char)temp);

                // This condition allows us to process events that spread over multiple lines
                if (IsEndWith(sb, log4jEndTag))
                {
                    LogMessage logMsg = ReceiverUtils.ParseLog4JXmlLogEvent(sb.ToString(), _fullLoggerName);
                    // Notify the UI with the set of messages
                    if (Notifiable != null)
                        Notifiable.Notify(logMsg);
                    sb = new StringBuilder();
                }
            }
        }
    }

    [Serializable]
    [DisplayName("Log File (PatternLayout)")]
    public class PatternLayoutFileReceiver : FileReceiver
    {
        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return @"Configuration for nlog: ( !!! DO NOT change layout. )
<target name='file' xsi:type='File' encoding='utf-8' fileName='${basedir}/log/patternFile.log' layout='${longdate} [${threadid}] ${pad:padding=-5:fixedlength=true:inner=${level:uppercase=true}} ${logger} - ${message}' />      

Configuration for log4net: ( !!! DO NOT change layout. )
<appender name='fileLog4j' type='log4net.Appender.FileAppender'>
    <file value='log/fileLog4j.log' />
    <encoding value='utf-8'></encoding>
    <appendToFile value='true' /> 
    <layout type='log4net.Layout.PatternLayout'>
      <conversionPattern value='%date [%thread] %-5level %logger - %message%newline'/>
    </layout>
</appender> 

OR

<appender name='rollingFileLog4j' type='log4net.Appender.RollingFileAppender,log4net' >
    <File value='log/rollingFileLog4j.log' />
    <Encoding value='utf-8' /> 
    <AppendToFile value='true' />
    <RollingStyle value='Composite' />
    <DatePattern value='.yyyy.MM.dd.'log'' />
    <MaximumFileSize value='20MB' />
    <maxSizeRollBackups value='10' />
    <!--file number increase after the file's size exceeds the MaximumFileSize.-->
    <CountDirection value='1' />
    <StaticLogFileName value='true' />
    <layout type='log4net.Layout.PatternLayout'>
      <conversionPattern value='%date [%thread] %-5level %logger - %message%newline'/>
    </layout>
</appender>
".Replace("'", "\"").Replace("\n", Environment.NewLine);
            }
        }

        protected override void ProcessFileData()
        {
            int temp;
            Parser parser = new Parser();
            while ((temp = fileReader.Read()) != -1)
            {
                var log = parser.parse((char)temp);
                if (log != null)
                {
                    if (Notifiable != null)
                        Notifiable.Notify(log);
                }
            }
            var logLast = parser.GetLog();
            if (logLast != null)
            {
                if (Notifiable != null)
                    Notifiable.Notify(logLast);
            }
        }
    }

    /// <summary>
    /// This receiver watch a given file, like a 'tail' program, with one log event by line.
    /// Ideally the log events should use the log4j XML Schema layout.
    /// </summary>
    [Serializable]
    [DisplayName("Log File (Log4j XML Formatted)")]
    public abstract class FileReceiver : BaseReceiver
    {
        [NonSerialized]
        private object locker;

        [NonSerialized]
        private FileSystemWatcher _fileWatcher;

        [NonSerialized]
        private long _lastFileLength;
        [NonSerialized]
        private string _filename;
        [NonSerialized]
        protected string _fullLoggerName;

        private string _fileToWatch = String.Empty;
        private bool _showFromBeginning = true;
        private string _loggerName;


        [Category("Configuration")]
        [DisplayName("File to Watch")]
        public string FileToWatch
        {
            get { return _fileToWatch; }
            set
            {
                if (String.Compare(_fileToWatch, value, true) == 0)
                    return;

                _fileToWatch = value;
            }
        }

        [Category("Configuration")]
        [DisplayName("Show from Beginning")]
        [Description("Show file contents from the beginning (not just newly appended lines)")]
        [DefaultValue(true)]
        public bool ShowFromBeginning
        {
            get { return _showFromBeginning; }
            set
            {
                _showFromBeginning = value;
            }
        }

        [Category("Behavior")]
        [DisplayName("Logger Name")]
        [Description("Append the given Name to the Logger Name. If left empty, the filename will be used.")]
        public string LoggerName
        {
            get { return _loggerName; }
            set
            {
                _loggerName = value;

                ComputeFullLoggerName();
            }
        }


        #region IReceiver Members 

        public override void Initialize()
        {
            if (String.IsNullOrEmpty(_fileToWatch))
                return;

            //NonSerialized field must be initilized here.
            locker = new object();
            _waitReadExistingLogs = new ManualResetEvent(false);

            string path = Path.GetDirectoryName(_fileToWatch);
            _filename = Path.GetFileName(_fileToWatch);
            _fileWatcher = new FileSystemWatcher(path, _filename);
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            ComputeFullLoggerName();
        }

        [NonSerialized]
        FileStream fileStream;

        [NonSerialized]
        protected StreamReader fileReader;

        [NonSerialized]
        ManualResetEvent _waitReadExistingLogs;

        public override void Start()
        {
            _fileWatcher.Changed += _fileWatcher_Changed;
            _fileWatcher.Deleted += _fileWatcher_Deleted;
            _fileWatcher.Renamed += _fileWatcher_Renamed;
            _fileWatcher.Created += _fileWatcher_Created;
            _fileWatcher.EnableRaisingEvents = true;
            _fileWatcher.NotifyFilter =
               (NotifyFilters)(383);

            // (FileShare.ReadWrite | FileShare.Delete) to allow log4net RollingFileAppender move original file to log.1, log.2 ... file.
            // And even after file is deleted (deleted, moved, renamed), the stream object can still read the file. Because file will only be really deleted after the stream closed.
            fileStream = new FileStream(_fileToWatch, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            fileReader = new StreamReader(fileStream, this.EncodingObject);

            if (_showFromBeginning)
            {
                ReadFile();
            }
            else
            {
                _lastFileLength = fileReader.BaseStream.Length;
            }

            _waitReadExistingLogs.Set();

            //If log4net use deault lock ExclusiveLock 
            // (has best performance and can make sure only one thread wirte file.)
            // File change event will not occur properly. So read file every second.
            ThreadPool.QueueUserWorkItem(delegate (object ob)
            {
                while (_fileWatcher != null)
                {
                    try
                    {
                        ReadFile();
                    }
                    catch (Exception ex)
                    {
                        Utils.log.Error(ex.Message, ex);
                    }
                    finally
                    {
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        private void _fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType != WatcherChangeTypes.Changed)
                    return;

                _waitReadExistingLogs.WaitOne();
                ReadFile();
            }
            catch (Exception ex)
            {
                Utils.log.Error(ex.Message, ex);
            }
        }

        private void _fileWatcher_Renamed(object sender, RenamedEventArgs e)
        {

        }

        private void _fileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {

        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            // lock to wait previous reading finished. 
            // even old file has been moved, opened stream can still read its data. 
            lock (locker)
            {
                //red remain content of old file.
                ReadFile();

                fileStream.Dispose();
                fileReader.Dispose();

                //Open new created file.
                fileStream = new FileStream(_fileToWatch, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                fileReader = new StreamReader(fileStream, this.EncodingObject);

                //log4net RollingFileAppender move old file and crate a new file.
                _lastFileLength = 0;
            }
        }

        public override void Terminate()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= _fileWatcher_Changed;
                _fileWatcher.Deleted -= _fileWatcher_Deleted;
                _fileWatcher.Renamed -= _fileWatcher_Renamed;
                _fileWatcher.Created -= _fileWatcher_Created;
                _fileWatcher.Dispose();
                _fileWatcher = null;
            }

            if (fileReader != null)
            {
                fileStream.Dispose();
                fileReader.Dispose();
                fileStream = null;
                fileReader = null;
            }

            _lastFileLength = 0;
        }

        #endregion

        private void ComputeFullLoggerName()
        {
            _fullLoggerName = String.Format("FileLogger.{0}",
                                            String.IsNullOrEmpty(_loggerName)
                                                ? _filename.Replace('.', '_')
                                                : _loggerName);

            DisplayName = String.IsNullOrEmpty(_loggerName)
                              ? String.Empty
                              : String.Format("Log File [{0}]", _loggerName);
        }

        protected abstract void ProcessFileData();

        private void ReadFile()
        {
            //Only allowed one thread to read the file.
            //OnFileChanged event may raise in multiple thread when the file changed very frenquently. 
            // _lastFileLength may be change to 0 after log4net RollingFileAppender move original file to log.1, log.2 ... file.
            lock (locker)
            {
                if ((fileReader == null) || (fileReader.BaseStream.Length == _lastFileLength))
                    return;

                // Seek to the last file length
                fileReader.BaseStream.Seek(_lastFileLength, SeekOrigin.Begin);

                ProcessFileData();

                // Update the last file length
                _lastFileLength = fileReader.BaseStream.Position;

            }
        }
    }

    enum State
    {
        Time,
        Thread,
        LevelAndLogger,
        Message
    }

    class Parser
    {
        State state = State.Time;

        public int newLineIndex = -1;
        public int bracketStartIndex = -1;
        public int bracketEndIndex = -1;
        public int dashIndex = -1;

        string getSub(string str, int start, int end)
        {
            var len = end + 1 - start;
            return str.Substring(start, len);
        }

        StringBuilder sb = new StringBuilder();
        public LogMessage parse(char ch)
        {
            LogMessage log = null;
            sb.Append(ch);
            switch (ch)
            {
                case '[':
                    if (state == State.Time)
                    {
                        state = State.Thread;
                        bracketStartIndex = sb.Length - 1;
                    }
                    else if (state == State.Message)
                    {
                        if (newLineIndex >= 0)
                        {
                            DateTime tempTime;
                            var strTime = getSub(sb.ToString(), newLineIndex + 1, sb.Length - 2).Replace(",", ".");
                            if (DateTime.TryParse(strTime, out tempTime))
                            {
                                log = GetLog(newLineIndex - 1);

                                state = State.Thread;
                                bracketStartIndex = sb.Length - 1;
                            }
                        }
                    }
                    break;
                case ']':
                    if (state == State.Thread)
                    {
                        state = State.LevelAndLogger;
                        bracketEndIndex = sb.Length - 1;
                    }

                    break;
                case '-':
                    if (state == State.LevelAndLogger)
                    {
                        state = State.Message;
                        dashIndex = sb.Length - 1;
                    }

                    break;
                case '\n':
                    newLineIndex = sb.Length - 1;
                    break;
                default:
                    break;
            }

            return log;
        }

        public LogMessage GetLog(int endIndex = -1)
        {
            if (endIndex == -1)
            {
                endIndex = sb.Length - 1;
            }
            var str = sb.ToString();
            LogMessage log = new LogMessage();

            try
            {
                log.TimeStamp = DateTime.Parse(getSub(str, 0, bracketStartIndex - 1).Trim().Replace(",", "."));
                log.ThreadName = getSub(str, bracketStartIndex + 1, bracketEndIndex - 1).Trim();
                ParseLevelAndLogger(str, log);
                log.Message = getSub(str, dashIndex + 1, endIndex).TrimStart();
            }
            catch (Exception ex)
            {
                Utils.log.Error(ex.Message, ex);
                if (sb.Length > 0)
                {
                    log.Message = str.Substring(0, Math.Min(str.Length, endIndex + 1));
                }
                else
                {
                    log = null;
                }
            }

            newLineIndex = -1;
            bracketStartIndex = -1;
            bracketEndIndex = -1;
            dashIndex = -1;

            if (endIndex + 1 < sb.Length)
            {
                sb = new StringBuilder(str.Substring(endIndex + 1));
            }
            else
            {
                sb.Clear();
            }
            return log;
        }

        private void ParseLevelAndLogger(string str, LogMessage log)
        {
            var subStr = getSub(str, bracketEndIndex + 1, dashIndex - 1).Trim();
            var blankIndex = subStr.IndexOf(' ');
            log.Level = LogLevels.Instance[getSub(subStr, 0, blankIndex).Trim()];
            log.LoggerName = subStr.Substring(blankIndex + 1).Trim();
        }
    }
}
