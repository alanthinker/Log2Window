using CsvHelper;
using CsvHelper.Configuration;
using Log2Window.Log;
using Log2Window.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Log2Window
{
    public static class Utils
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("Log2Window");
        static readonly DateTime s1970 = new DateTime(1970, 1, 1);

        private static void WriteARow(CsvWriter csvWriter, LogMessage logMsg)
        {
            string temp = null;
            //Add all the Standard Fields to the ListViewItem
            for (int i = 0; i < UserSettings.Instance.ColumnConfiguration.Length; i++)
            {
                switch (UserSettings.Instance.ColumnConfiguration[i].Field)
                {
                    case LogMessageField.SequenceNr:
                        temp = logMsg.SequenceNr.ToString();
                        break;
                    case LogMessageField.ArrivedId:
                        temp = logMsg.ArrivedId.ToString();
                        break;
                    case LogMessageField.LoggerName:
                        temp = logMsg.LoggerName;
                        break;
                    case LogMessageField.RootLoggerName:
                        temp = logMsg.RootLoggerName;
                        break;
                    case LogMessageField.Level:
                        temp = logMsg.Level.Name;
                        break;
                    case LogMessageField.Message:
                        string msg = logMsg.Message.Replace("\r\n", " ");
                        msg = msg.Replace("\n", " ");
                        temp = msg;
                        break;
                    case LogMessageField.ThreadName:
                        temp = logMsg.ThreadName;
                        break;
                    case LogMessageField.TimeStamp:
                        temp = logMsg.TimeStamp.ToString(UserSettings.Instance.TimeStampFormatString);
                        break;
                    case LogMessageField.Exception:
                        string exception = logMsg.ExceptionString.Replace("\r\n", " ");
                        exception = exception.Replace("\n", " ");
                        temp = exception;
                        break;
                    case LogMessageField.CallSiteClass:
                        temp = logMsg.CallSiteClass;
                        break;
                    case LogMessageField.CallSiteMethod:
                        temp = logMsg.CallSiteMethod;
                        break;
                    case LogMessageField.SourceFileName:
                        temp = logMsg.SourceFileName;
                        break;
                    case LogMessageField.SourceFileLineNr:
                        temp = logMsg.SourceFileLineNr.ToString();
                        break;
                    case LogMessageField.Properties:
                        break;
                }
                csvWriter.WriteField(temp);
            }
            csvWriter.NextRecord();
        }

        public static void Export2Excel(string FileName)
        {
            lock (LogManager.Instance.dataLocker)
            {
                using (StreamWriter textWriter = new StreamWriter(FileName, false, Encoding.UTF8))
                {
                    var csvWriter = new CsvWriter(textWriter);
                    csvWriter.Configuration.Encoding = Encoding.UTF8;

                    for (int i = 0; i < UserSettings.Instance.ColumnConfiguration.Length; i++)
                    {
                        csvWriter.WriteField(UserSettings.Instance.ColumnConfiguration[i].Field.ToString());
                    }
                    csvWriter.NextRecord();

                    foreach (var loggerItem in LogManager.Instance._dataSource)
                    {
                        WriteARow(csvWriter, loggerItem.Message);
                    }
                }
            }

            FileInfo fil = new FileInfo(FileName);
            if (fil.Exists == true)
                MessageBox.Show("Process Completed", "Export to Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public static void Export2Log4jFile(string FileName)
        {
            lock (LogManager.Instance.dataLocker)
            {
                using (var streamWriter = new StreamWriter(FileName, false, Encoding.UTF8))
                using (XmlWriter textWriter = new XmlTextWriter(streamWriter))
                {
                    foreach (var loggerItem in LogManager.Instance._dataSource)
                    {
                        WriteLog4jxmlItem(textWriter, loggerItem.Message);
                        streamWriter.WriteLine();
                    }
                }
            }

            FileInfo fil = new FileInfo(FileName);
            if (fil.Exists == true)
                MessageBox.Show("Process Completed", "Export to Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public static void WriteLog4jxmlItem(XmlWriter writer, LogMessage logMsg)
        {
            writer.WriteStartElement("log4j:event");

            writer.WriteAttributeString("logger", logMsg.LoggerName);
            writer.WriteAttributeString("level", (logMsg.Level.Level).ToString());
            writer.WriteAttributeString("thread", logMsg.ThreadName);
            writer.WriteAttributeString("timestamp", (logMsg.TimeStamp - s1970).TotalMilliseconds.ToString());

            writer.WriteElementString("log4j:message", logMsg.Message);
            if (!string.IsNullOrEmpty(logMsg.ExceptionString))
            {
                writer.WriteElementString("log4j:throwable", logMsg.ExceptionString);
            }
            if (!string.IsNullOrEmpty(logMsg.CallSiteClass))
            {
                writer.WriteStartElement("log4j:locationInfo");
                writer.WriteAttributeString("class", logMsg.CallSiteClass);
                writer.WriteAttributeString("method", logMsg.CallSiteMethod);
                writer.WriteAttributeString("file", logMsg.SourceFileName);
                if (logMsg.SourceFileLineNr != null)
                {
                    writer.WriteAttributeString("line", logMsg.SourceFileLineNr?.ToString());
                }
                writer.WriteEndElement();
            }

            if (logMsg.Properties != null)
            {
                writer.WriteStartElement("log4j:properties");
                foreach (var property in logMsg.Properties)
                {
                    writer.WriteStartElement("log4j:data");
                    writer.WriteAttributeString("name", property.Key);
                    writer.WriteAttributeString("value", property.Value);
                    writer.WriteEndElement();

                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public static DateTime GetPeTime(string fileName)
        {
            int seconds;
            using (var br = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
            {
                var bs = br.ReadBytes(2);
                var msg = "非法的PE文件";
                if (bs.Length != 2) throw new Exception(msg);
                if (bs[0] != 'M' || bs[1] != 'Z') throw new Exception(msg);
                br.BaseStream.Seek(0x3c, SeekOrigin.Begin);
                var offset = br.ReadByte();
                br.BaseStream.Seek(offset, SeekOrigin.Begin);
                bs = br.ReadBytes(4);
                if (bs.Length != 4) throw new Exception(msg);
                if (bs[0] != 'P' || bs[1] != 'E' || bs[2] != 0 || bs[3] != 0) throw new Exception(msg);
                bs = br.ReadBytes(4);
                if (bs.Length != 4) throw new Exception(msg);
                seconds = br.ReadInt32();
            }
            return DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc).
              AddSeconds(seconds).ToLocalTime();
        }
    }
}