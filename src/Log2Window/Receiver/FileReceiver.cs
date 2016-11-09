using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using Log2Window.Log;
using System.Threading;

namespace Log2Window.Receiver
{
    /// <summary>
    /// This receiver watch a given file, like a 'tail' program, with one log event by line.
    /// Ideally the log events should use the log4j XML Schema layout.
    /// </summary>
    [Serializable]
    [DisplayName("Log File (Log4j XML Formatted)")]
    public class FileReceiver : BaseReceiver
    {
        [NonSerialized]
        private object locker;

        public enum FileFormatEnums
        {
            Log4jXml,
            //Flat,
        }


        [NonSerialized]
        private FileSystemWatcher _fileWatcher;

        [NonSerialized]
        private long _lastFileLength;
        [NonSerialized]
        private string _filename;
        [NonSerialized]
        private string _fullLoggerName;

        private string _fileToWatch = String.Empty;
        private FileFormatEnums _fileFormat;
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

        public override void Initialize()
        {
            if (String.IsNullOrEmpty(_fileToWatch))
                return;

            //NonSerialized field must be initilized here.
            locker = new object();
            _waitReadExistingLogs = new ManualResetEvent(false);
            sb = new StringBuilder();

            string path = Path.GetDirectoryName(_fileToWatch);
            _filename = Path.GetFileName(_fileToWatch);
            _fileWatcher = new FileSystemWatcher(path, _filename);
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            ComputeFullLoggerName();
        }

        [NonSerialized]
        ManualResetEvent _waitReadExistingLogs;
        public override void Start()
        {
            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.Deleted += _fileWatcher_Deleted;
            _fileWatcher.Created += _fileWatcher_Created;
            _fileWatcher.EnableRaisingEvents = true;
            _fileWatcher.NotifyFilter =
               (NotifyFilters)(383);

            if (_showFromBeginning)
            {
                ReadFile();
            }
            else
            { 
                using (var stream = new FileStream(_fileToWatch, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (var fileReader = new StreamReader(stream, this.EncodingObject))
                {
                    _lastFileLength = fileReader.BaseStream.Length;
                }
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

        private void _fileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {

        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            // lock to wait previous reading finished. 
            // even old file has been moved, opened stream can still read its data.
            lock (locker)
            {
                //log4net RollingFileAppender move old file and crate a new file.
                _lastFileLength = 0;
            }
        }

        public override void Terminate()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= OnFileChanged;
                _fileWatcher = null;
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

        static char[] log4jEndTag = "</log4j:event>".ToCharArray();
        private void OnFileChanged(object sender, FileSystemEventArgs e)
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

        //if log4net set ImmediateFlush=false, log message may only write only a part.
        //So need stock this part.
        [NonSerialized]
        StringBuilder sb = new StringBuilder();

        private void ReadFile()
        {
            //Only allowed one thread to read the file.
            //OnFileChanged event may raise in multiple thread when the file changed very frenquently. 
            // _lastFileLength may be change to 0 after log4net RollingFileAppender move original file to log.1, log.2 ... file.
            lock (locker)
            {
                // (FileShare.ReadWrite | FileShare.Delete) to allow log4net RollingFileAppender move original file to log.1, log.2 ... file.
                // And even after file is deleted (deleted, moved, renamed), the stream object can still read the file. Because file will only be really deleted after the stream closed.
                using (var stream = new FileStream(_fileToWatch, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (var fileReader = new StreamReader(stream, this.EncodingObject))
                {
                    if ((fileReader == null) || (fileReader.BaseStream.Length == _lastFileLength))
                        return;

                    // Seek to the last file length
                    fileReader.BaseStream.Seek(_lastFileLength, SeekOrigin.Begin);

                    // Get last added lines
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

                    // Update the last file length
                    _lastFileLength = fileReader.BaseStream.Position;
                }
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
    }
}
