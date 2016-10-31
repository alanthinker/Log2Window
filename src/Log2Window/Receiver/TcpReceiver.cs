using Log2Window.Log;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Log2Window.Receiver
{
    [Serializable]
    [DisplayName("TCP (IP v4 and v6)")]
    public class TcpReceiver : BaseReceiver
    {
        #region Port Property

        int _port = 4505;
        [Category("Configuration")]
        [DisplayName("TCP Port Number")]
        [DefaultValue(4505)]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        #endregion

        #region IpV6 Property

        bool _ipv6;
        [Category("Configuration")]
        [DisplayName("Use IPv6 Addresses")]
        [DefaultValue(false)]
        public bool IpV6
        {
            get { return _ipv6; }
            set { _ipv6 = value; }
        }

        private int _bufferSize = 10000;
        [Category("Configuration")]
        [DisplayName("Receive Buffer Size")]
        [DefaultValue(10000)]
        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }

        #endregion

        #region IReceiver Members

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return
                    "Configuration for NLog:" + Environment.NewLine +
                    "<target name=\"TcpOutlet\" xsi:type=\"NLogViewer\" address=\"tcp://localhost:4505\"/>\r\n\r\n"
                    + "Configuration for log4net:" + Environment.NewLine +
                    "    <appender name=\"tcpAppender\" type=\"AlanThinker.MyLog4net.TcpAppender\">\r\n      <remoteAddress value=\"127.0.0.1\" />\r\n      <remotePort value=\"4505\" />\r\n      <encoding value=\"utf-8\"></encoding>\r\n      <layout type=\"log4net.Layout.XmlLayoutSchemaLog4j\" >\r\n        <locationInfo value=\"true\" />\r\n      </layout>\r\n    </appender>";
            }
        }

        [NonSerialized]
        Socket _socket;

        public override void Initialize()
        {
            if (_socket != null) return;

            _socket = new Socket(_ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.ExclusiveAddressUse = true;
            _socket.Bind(new IPEndPoint(_ipv6 ? IPAddress.IPv6Any : IPAddress.Any, _port));
            _socket.Listen(100);
            _socket.ReceiveBufferSize = _bufferSize;
            var args = new SocketAsyncEventArgs();
            args.Completed += AcceptAsyncCompleted;

            _socket.AcceptAsync(args);
        }

        void AcceptAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (_socket == null || e.SocketError != SocketError.Success) return;

                //Must start a new thread to prcess data, otherwise can only process only one connection.
                new Thread(ProcessReceivedData) { IsBackground = true }.Start(e.AcceptSocket);

                e.AcceptSocket = null;
                _socket.AcceptAsync(e);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        static char[] log4jEndTag = "</log4j:event>".ToCharArray();
        void ProcessReceivedData(object newSocket)
        {
            try
            {
                using (var socket = (Socket)newSocket)
                using (var ns = new NetworkStream(socket, FileAccess.Read, false))
                    while (_socket != null)
                    {
                        using (StreamReader sr = new StreamReader(ns))
                        {
                            //NetworkStream may contain multiple log4j:event, if the tcp send message very frequently.
                            StringBuilder sb = new StringBuilder();

                            int temp;
                            while ((temp = sr.Read()) != -1)
                            {
                                sb.Append((char)temp);
                                if (IsEndWith(sb, log4jEndTag))
                                {
                                    var str = sb.ToString();
                                    LogMessage logMsg = ReceiverUtils.ParseLog4JXmlLogEvent(str, "TcpLogger");
                                    logMsg.RootLoggerName = logMsg.LoggerName;
                                    //logMsg.LoggerName = string.Format(":{1}.{0}", logMsg.LoggerName, _port); 

                                    var ipEndPoint = socket.RemoteEndPoint as IPEndPoint;
                                    if (ipEndPoint != null)
                                    {
                                        logMsg.LoggerName = string.Format("{0}.{1}", ipEndPoint.Address.ToString().Replace('.', '_'), logMsg.LoggerName);
                                    }
                                    else
                                    {
                                        var dnsEndPoint = socket.RemoteEndPoint as DnsEndPoint;
                                        logMsg.LoggerName = string.Format("{0}.{1}", dnsEndPoint.Host.Replace('.', '_'), logMsg.LoggerName);
                                    }

                                    if (Notifiable != null)
                                        Notifiable.Notify(logMsg);

                                    sb = new StringBuilder();
                                } 

                            }
                        }
                    }
            }
            catch (IOException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        public override void Terminate()
        {
            if (_socket == null) return;

            _socket.Close();
            _socket = null;
        }

        #endregion
    }
}
