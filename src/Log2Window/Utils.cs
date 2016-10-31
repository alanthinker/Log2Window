using CsvHelper;
using CsvHelper.Configuration;
using Log2Window.Log;
using Log2Window.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace Log2Window
{
    public static class utils
    {
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

        public static void Export2Excel(ListView listView, string FileName)
        {
            lock (LogManager.Instance.dataLocker)
            {
                using (StreamWriter textWriter = new StreamWriter(FileName, false))
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
    }
}