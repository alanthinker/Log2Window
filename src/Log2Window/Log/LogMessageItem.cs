﻿using System.Text;
using Log2Window.Settings;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Log2Window.Log
{
    public static class IdCreator
    {
        private static readonly object idLocker = new object();

        private static ulong id = 1;
        public static ulong GetNextId()
        {
            lock (idLocker)
            {
                return id++;
            }
        }
    }

    /// <summary>
    /// Describes a Log Message.
    /// TODO: Make it disposable to dereference Item?
    /// </summary>
    public class LogMessageItem : IComparable<LogMessageItem>
    {
        /// <summary>
        /// Logger Item Parent.
        /// </summary>
        public LoggerItem Parent;

        ///// <summary>
        ///// The item before this one, allow to retrieve the order of arrival (time is not reliable here).
        ///// The previous item is not necessary a sibling in the logger tree, only in the message list view.
        ///// </summary>
        //public LogMessageItem Previous;

        // public long ArrivedId;

        ///// <summary>
        ///// The associated List View Item.
        ///// </summary>
        //public ListViewItem Item;

        /// <summary>
        /// Log Message.
        /// </summary>
        public LogMessage Message;

        /// <summary>
        /// Indicates if this Log Message Item is enable.
        /// When disabled the List View Item is not in the Log List View.
        /// </summary>
        public bool Enabled = true;

        public LogMessageItem(LoggerItem parent, LogMessage logMsg)
        {
            Parent = parent;
            Message = logMsg;
        }

        public static ListViewItem CreateListViewItem(LogMessage logMsg)
        {
            Utils.log.Debug("CreateListViewItem " + logMsg.ArrivedId);
            // Create List View Item
            var items = new ListViewItem.ListViewSubItem[UserSettings.Instance.ColumnConfiguration.Length];
            string toolTip = string.Empty;

            //Add all the Standard Fields to the ListViewItem
            for (int i = 0; i < UserSettings.Instance.ColumnConfiguration.Length; i++)
            {
                items[i] = new ListViewItem.ListViewSubItem();

                switch (UserSettings.Instance.ColumnConfiguration[i].Field)
                {
                    case LogMessageField.SequenceNr:
                        items[i].Text = logMsg.SequenceNr.ToString();
                        break;
                    case LogMessageField.ArrivedId:
                        items[i].Text = logMsg.ArrivedId.ToString();
                        break;
                    case LogMessageField.LoggerName:
                        items[i].Text = logMsg.LoggerName;
                        break;
                    case LogMessageField.RootLoggerName:
                        items[i].Text = logMsg.RootLoggerName;
                        break;
                    case LogMessageField.Level:
                        items[i].Text = logMsg.Level.Name;
                        break;
                    case LogMessageField.Message:
                        StringBuilder sbMsg = new StringBuilder(logMsg.Message);
                        if (!string.IsNullOrEmpty(logMsg.ExceptionString))
                        {
                            sbMsg.Append(" " + logMsg.ExceptionString);
                        }
                        sbMsg.Replace("\r\n", " ");
                        sbMsg.Replace("\n", " ");
                        var msg = sbMsg.ToString();
                        items[i].Text = msg;
                        toolTip = msg;
                        break;
                    case LogMessageField.ThreadName:
                        items[i].Text = logMsg.ThreadName;
                        break;
                    case LogMessageField.TimeStamp:
                        items[i].Text = logMsg.TimeStamp.ToString(UserSettings.Instance.TimeStampFormatString);
                        break;
                    case LogMessageField.Exception:
                        string exception = logMsg.ExceptionString.Replace("\r\n", " ");
                        exception = exception.Replace("\n", " ");
                        items[i].Text = exception;
                        break;
                    case LogMessageField.CallSiteClass:
                        items[i].Text = logMsg.CallSiteClass;
                        break;
                    case LogMessageField.CallSiteMethod:
                        items[i].Text = logMsg.CallSiteMethod;
                        break;
                    case LogMessageField.SourceFileName:
                        items[i].Text = logMsg.SourceFileName;
                        break;
                    case LogMessageField.SourceFileLineNr:
                        items[i].Text = logMsg.SourceFileLineNr.ToString();
                        break;
                    case LogMessageField.Properties:
                        break;
                }
            }

            //Add all the Properties in the Message to the ListViewItem
            foreach (var property in logMsg.Properties)
            {
                string propertyKey = property.Key;
                if (UserSettings.Instance.ColumnProperties.ContainsKey(propertyKey))
                {
                    int propertyColumnNumber = UserSettings.Instance.ColumnProperties[propertyKey];
                    if (propertyColumnNumber < items.Length)
                    {
                        items[propertyColumnNumber].Text = property.Value;
                    }
                }
            }

            var Item = new ListViewItem(items, 0) { ToolTipText = toolTip, ForeColor = logMsg.Level.Color };//, Tag = this };
            return Item;
        }

        internal bool IsLevelInRange()
        {
            return (Message.Level.RangeMax >= UserSettings.Instance.LogLevelInfo.RangeMax);
        }

        //internal void HighlightSearchedText(bool hasText, string str)
        //{
        //    if (hasText && HasSearchedText(str))
        //        Item.BackColor = Color.LightYellow;
        //    else
        //        Item.BackColor = Color.Transparent;
        //}

        internal bool HasSearchedText(string str)
        {
            return (Message.Message.IndexOf(str, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        internal bool IsThreadMatch(string[] threads)
        {
            return threads.Contains(Message.ThreadName);
        }

        internal void GetMessageDetails(RichTextBox logDetailTextBox, RichTextBox tbMessage)
        {
            Message.GetMessageDetails(logDetailTextBox, tbMessage);
        }

        public int CompareTo(LogMessageItem other)
        {
            return this.Message.ArrivedId.CompareTo(other.Message.ArrivedId);            
        }
    }
}
