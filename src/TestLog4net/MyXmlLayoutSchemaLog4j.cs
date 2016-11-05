using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using log4net.Core;
using log4net.Util;

namespace AlanThinker.MyLog4net
{
    /// <summary>
    /// SimpleXmlLayoutSchemaLog4j which without hostname, appdomain name, identity name, user name, ndcObj and properties.
    /// So have better performance.
    /// </summary>
    class MyXmlLayoutSchemaLog4j : log4net.Layout.XmlLayoutSchemaLog4j
    {
        private static readonly DateTime s_date1970 = new DateTime(1970, 1, 1);

        #region Some switch which can cause performance issue.

        public bool Show_Hostname_Appdomain_Identity_UserName { get; set; }
        public bool ShowNDC { get; set; }
        public bool ShowProperties { get; set; }

        #endregion  

        protected override void FormatXml(XmlWriter writer, LoggingEvent loggingEvent)
        {
            //// Translate logging events for log4j

            if (this.Show_Hostname_Appdomain_Identity_UserName)
            {
                // Translate hostname property
                if (loggingEvent.LookupProperty(LoggingEvent.HostNameProperty) != null &&
                    loggingEvent.LookupProperty("log4jmachinename") == null)
                {
                    loggingEvent.GetProperties()["log4jmachinename"] = loggingEvent.LookupProperty(LoggingEvent.HostNameProperty);
                }

                // translate appdomain name
                if (loggingEvent.LookupProperty("log4japp") == null &&
                    loggingEvent.Domain != null &&
                    loggingEvent.Domain.Length > 0)
                {
                    loggingEvent.GetProperties()["log4japp"] = loggingEvent.Domain;
                }

                // translate identity name
                if (loggingEvent.Identity != null &&
                    loggingEvent.Identity.Length > 0 &&
                    loggingEvent.LookupProperty(LoggingEvent.IdentityProperty) == null)
                {
                    loggingEvent.GetProperties()[LoggingEvent.IdentityProperty] = loggingEvent.Identity;
                }

                // translate user name
                if (loggingEvent.UserName != null &&
                    loggingEvent.UserName.Length > 0 &&
                    loggingEvent.LookupProperty(LoggingEvent.UserNameProperty) == null)
                {
                    loggingEvent.GetProperties()[LoggingEvent.UserNameProperty] = loggingEvent.UserName;
                }
            } 

            // Write the start element
            writer.WriteStartElement("log4j:event");
            writer.WriteAttributeString("logger", loggingEvent.LoggerName);

            // Calculate the timestamp as the number of milliseconds since january 1970
            // 
            // We must convert the TimeStamp to UTC before performing any mathematical
            // operations. This allows use to take into account discontinuities
            // caused by daylight savings time transitions.
            TimeSpan timeSince1970 = loggingEvent.TimeStamp.ToUniversalTime() - s_date1970;

            writer.WriteAttributeString("timestamp", XmlConvert.ToString((long)timeSince1970.TotalMilliseconds));
            writer.WriteAttributeString("level", loggingEvent.Level.DisplayName);
            writer.WriteAttributeString("thread", loggingEvent.ThreadName);

            // Append the message text
            writer.WriteStartElement("log4j:message");
            Transform.WriteEscapedXmlString(writer, loggingEvent.RenderedMessage, this.InvalidCharReplacement);
            writer.WriteEndElement();

            if (this.ShowNDC)
            {
                object ndcObj = loggingEvent.LookupProperty("NDC");
                if (ndcObj != null)
                {
                    string valueStr = loggingEvent.Repository.RendererMap.FindAndRender(ndcObj);

                    if (valueStr != null && valueStr.Length > 0)
                    {
                        // Append the NDC text
                        writer.WriteStartElement("log4j:NDC");
                        Transform.WriteEscapedXmlString(writer, valueStr, this.InvalidCharReplacement);
                        writer.WriteEndElement();
                    }
                }
            }

            // Append the properties text

            if (this.ShowProperties)
            {
                PropertiesDictionary properties = loggingEvent.GetProperties();
                if (properties.Count > 0)
                {
                    writer.WriteStartElement("log4j:properties");
                    foreach (System.Collections.DictionaryEntry entry in properties)
                    {
                        writer.WriteStartElement("log4j:data");
                        writer.WriteAttributeString("name", (string)entry.Key);

                        // Use an ObjectRenderer to convert the object to a string
                        string valueStr = loggingEvent.Repository.RendererMap.FindAndRender(entry.Value);
                        writer.WriteAttributeString("value", valueStr);

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
            } 

            string exceptionStr = loggingEvent.GetExceptionString();
            if (exceptionStr != null && exceptionStr.Length > 0)
            {
                // Append the stack trace line
                writer.WriteStartElement("log4j:throwable");
                Transform.WriteEscapedXmlString(writer, exceptionStr, this.InvalidCharReplacement);
                writer.WriteEndElement();
            }

            if (LocationInfo)
            {
                LocationInfo locationInfo = loggingEvent.LocationInformation;

                writer.WriteStartElement("log4j:locationInfo");
                writer.WriteAttributeString("class", locationInfo.ClassName);
                writer.WriteAttributeString("method", locationInfo.MethodName);
                writer.WriteAttributeString("file", locationInfo.FileName);
                writer.WriteAttributeString("line", locationInfo.LineNumber);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
