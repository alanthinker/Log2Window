using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Log2Window.Settings;
using System.Diagnostics;

namespace Log2Window.Log
{
    public class LogMessage
    {
        /// <summary>
        /// The Line Number of the Log Message
        /// </summary>
        public ulong SequenceNr;

        /// <summary>
        /// Logger Name.
        /// </summary>
        public string LoggerName;

        /// <summary>
        /// Root Logger Name.
        /// </summary>
        public string RootLoggerName;

        /// <summary>
        /// Log Level.
        /// </summary>
        public LogLevelInfo Level;

        /// <summary>
        /// Log Message.
        /// </summary>
        public string Message;

        /// <summary>
        /// Thread Name.
        /// </summary>
        public string ThreadName;

        /// <summary>
        /// Time Stamp.
        /// </summary>
        public DateTime TimeStamp;

        /// <summary>
        /// Properties collection.
        /// </summary>
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        /// <summary>
        /// An exception message to associate to this message.
        /// </summary>
        public string ExceptionString;

        /// <summary>
        /// The CallSite Class
        /// </summary>
        public string CallSiteClass;


        /// <summary>
        /// The CallSite Method in which the Log is made
        /// </summary>
        public string CallSiteMethod;

        /// <summary>
        /// The Name of the Source File
        /// </summary>
        public string SourceFileName;

        /// <summary>
        /// The Line of the Source File
        /// </summary>
        public uint? SourceFileLineNr;

        //Only allowed set ArrivedId in constructor.
        public readonly ulong ArrivedId;

        public LogMessage()
        {
            ArrivedId = IdCreator.GetNextId();
        }

        public void CheckNull()
        {
            if (string.IsNullOrEmpty(LoggerName))
                LoggerName = "Unknown";
            if (string.IsNullOrEmpty(RootLoggerName))
                RootLoggerName = "Unknown";
            if (string.IsNullOrEmpty(Message))
                Message = "Unknown";
            if (string.IsNullOrEmpty(ThreadName))
                ThreadName = string.Empty;
            if (string.IsNullOrEmpty(ExceptionString))
                ExceptionString = string.Empty;
            if (string.IsNullOrEmpty(ExceptionString))
                ExceptionString = string.Empty;
            if (string.IsNullOrEmpty(CallSiteClass))
                CallSiteClass = string.Empty;
            if (string.IsNullOrEmpty(CallSiteMethod))
                CallSiteMethod = string.Empty;
            if (string.IsNullOrEmpty(SourceFileName))
                SourceFileName = string.Empty;
            if (Level == null)
                Level = LogLevels.Instance[(LogLevel.Error)];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var fieldType in UserSettings.Instance.ColumnConfiguration)
            {
                sb.Append(GetInformation(fieldType));
                sb.Append("\t");
            }
            return sb.ToString();
        }

        private string GetInformation(FieldType fieldType)
        {
            string result = string.Empty;
            switch (fieldType.Field)
            {
                case LogMessageField.SequenceNr:
                    result = SequenceNr.ToString();
                    break;
                case LogMessageField.ArrivedId:
                    result = ArrivedId.ToString();
                    break;
                case LogMessageField.LoggerName:
                    result = LoggerName;
                    break;
                case LogMessageField.RootLoggerName:
                    result = RootLoggerName;
                    break;
                case LogMessageField.Level:
                    result = Level.Level.ToString();
                    break;
                case LogMessageField.Message:
                    result = Message;
                    break;
                case LogMessageField.ThreadName:
                    result = ThreadName;
                    break;
                case LogMessageField.TimeStamp:
                    result = TimeStamp.ToString(UserSettings.Instance.TimeStampFormatString);
                    break;
                case LogMessageField.Exception:
                    result = ExceptionString;
                    break;
                case LogMessageField.CallSiteClass:
                    result = CallSiteClass;
                    break;
                case LogMessageField.CallSiteMethod:
                    result = CallSiteMethod;
                    break;
                case LogMessageField.SourceFileName:
                    result = SourceFileName;
                    break;
                case LogMessageField.SourceFileLineNr:
                    result = SourceFileLineNr.ToString();
                    break;
                case LogMessageField.Properties:
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (var property in Properties)
                        {
                            sb.Append(property.Key + ": ");
                            sb.AppendLine(property.Value);
                        }
                        result = sb.ToString(); 
                    }
                    
                    break;
            }
            return result;
        } 

        internal void GetMessageDetails(RichTextBox logDetailTextBox, RichTextBox tbMessage)
        {
            logDetailTextBox.Clear();
            foreach (var fieldType in UserSettings.Instance.MessageDetailConfiguration)
            {
                //var info = GetInformation(fieldType).Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
                //sb.Append(@"\b " + fieldType.Field + @": \b0 ");
                //if (info.Length > 40)
                //    sb.Append(@" \line ");
                //sb.Append(info + @" \line ");

                var info = GetInformation(fieldType);

                if (fieldType.Field == LogMessageField.Message)
                {
                    Utils.log.Debug(info);
                    tbMessage.Text = info;

                    if (!string.IsNullOrEmpty(this.ExceptionString))
                    {
                        tbMessage.Text += "\n\n" + this.ExceptionString;
                    }
                } 
                else
                {
                    var oldColor = logDetailTextBox.SelectionColor;
                    logDetailTextBox.SelectionColor = System.Drawing.Color.Brown;
                    logDetailTextBox.SelectionFont = new System.Drawing.Font(logDetailTextBox.SelectionFont, System.Drawing.FontStyle.Bold);
                    logDetailTextBox.AppendText(fieldType.Field + ": ");
                    logDetailTextBox.SelectionFont = new System.Drawing.Font(logDetailTextBox.SelectionFont, System.Drawing.FontStyle.Regular);
                    logDetailTextBox.SelectionColor = oldColor;
                    if (info.Length > 40)
                        logDetailTextBox.AppendText("\n");
                    logDetailTextBox.AppendText(info + "\n");
                }
            }
        }
    }
}
