using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Log2Window.Log;


namespace Log2Window.Receiver
{
    [Serializable]
    [DisplayName("UDP (IP v4 and v6)")]
    public class UdpReceiver : BaseReceiver
    {
        [NonSerialized]
        private Thread _worker;
        [NonSerialized]
        private UdpClient _udpClient;
        [NonSerialized]
        private IPEndPoint _remoteEndPoint;

        private bool _ipv6;
        private int _port = 7071;
        private string _address = String.Empty;
        private int _bufferSize = 1310720;


        [Category("Configuration")]
        [DisplayName("UDP Port Number")]
        [DefaultValue(7071)]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        [Category("Configuration")]
        [DisplayName("Use IPv6 Addresses")]
        [DefaultValue(false)]
        public bool IpV6
        {
            get { return _ipv6; }
            set { _ipv6 = value; }
        }

        [Category("Configuration")]
        [DisplayName("Multicast Group Address (Optional)")]
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        [Category("Configuration")]
        [DisplayName("Receive Buffer Size")]
        [DefaultValue(1310720)]
        public int BufferSize
        {
            get { return _bufferSize; }
            // UDP is not a reliable protocol. So increase the BufferSize to reduce the risk of lost packet.
            set { _bufferSize = Math.Max(1310720, value); }
        }

        bool _useRemoteIPAsNamespacePrefix;
        [Category("Configuration")]
        [DisplayName("Use Remote IP As Namespace Prefix.")]
        [DefaultValue(false)]
        public bool UseRemoteIPAsNamespacePrefix
        {
            get { return _useRemoteIPAsNamespacePrefix; }
            set { _useRemoteIPAsNamespacePrefix = value; }
        }

        #region IReceiver Members

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return
                    @"Notice! 
UDP is not a reliable protocol.
So recommend using AlanThinker.MyLog4net.TcpAppender.cs in the ExampleProject\TestLog4net project.

Configuration for NLog:
<target name='udp' xsi:type='NLogViewer' encoding='utf-8' address='udp4://localhost:7071' />

Configuration for log4net: 
<appender name='UdpAppender' type='log4net.Appender.UdpAppender'>
	<remoteAddress value='127.0.0.1' />
	<remotePort value='7071' />
	<encoding value='utf-8'></encoding>
	<layout type='AlanThinker.MyLog4net.MyXmlLayoutSchemaLog4j' /> 
</appender>
".Replace("'", "\"").Replace("\n", Environment.NewLine);
            }
        }

        public override void Initialize()
        {
            if ((_worker != null) && _worker.IsAlive)
                return;

            // Init connexion here, before starting the thread, to know the status now
            _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _udpClient = _ipv6 ? new UdpClient(_port, AddressFamily.InterNetworkV6) : new UdpClient(_port);
            _udpClient.Client.ReceiveBufferSize = _bufferSize;
            if (!String.IsNullOrEmpty(_address))
                _udpClient.JoinMulticastGroup(IPAddress.Parse(_address)); 
        }

        public override void Start()
        {
            // We need a working thread
            _worker = new Thread(StartUdp);
            _worker.IsBackground = true;
            _worker.Start();
        }

        public override void Terminate()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient = null;

                _remoteEndPoint = null;
            }

            if ((_worker != null) && _worker.IsAlive)
                _worker.Abort();
            _worker = null;
        }

        #endregion

        public void Clear()
        {
        }

        private void StartUdp()
        {
            while ((_udpClient != null) && (_remoteEndPoint != null))
            {
                try
                {
                    byte[] buffer = _udpClient.Receive(ref _remoteEndPoint);
                    string loggingEvent = this.EncodingObject.GetString(buffer);

                    //Console.WriteLine(loggingEvent);
                    //  Console.WriteLine("Count: " + count++);

                    if (Notifiable == null)
                        continue;

                    LogMessage logMsg = ReceiverUtils.ParseLog4JXmlLogEvent(loggingEvent, "UdpLogger");
                    if (_useRemoteIPAsNamespacePrefix)
                    {
                        logMsg.RootLoggerName = _remoteEndPoint.Address.ToString().Replace(".", "-");
                        logMsg.LoggerName = string.Format("{0}_{1}", _remoteEndPoint.Address.ToString().Replace(".", "-"), logMsg.LoggerName);
                    }
                   
                    if (Notifiable != null)
                        Notifiable.Notify(logMsg);
                }
                catch (ThreadAbortException ex)
                {
                    Utils.log.Error("StartUdp " + ex.Message);
                    Thread.ResetAbort();
                    break;
                }
                catch (Exception ex)
                {
                    Utils.log.Error(ex.Message, ex);
                    return;
                }
            }
        }

    }
}
