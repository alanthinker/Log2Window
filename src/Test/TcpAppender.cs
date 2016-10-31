using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

using log4net.Layout;
using log4net.Core;
using log4net.Util;
using log4net.Appender;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace AlanThinker.MyLog4net
{
    public class TcpAppender : AppenderSkeleton
    {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpAppender" /> class.
        /// </summary>
        /// <remarks>
        /// The default constructor initializes all fields to their default values.
        /// </remarks>
        public TcpAppender()
        {

        }

        #endregion Public Instance Constructors

        #region Public Instance Properties


        public IPAddress RemoteAddress
        {
            get { return m_remoteAddress; }
            set { m_remoteAddress = value; }
        }


        private int m_MaxQueueItemCount = 10000;
        public int MaxQueueItemCount
        {
            get { return m_MaxQueueItemCount; }
            set { m_MaxQueueItemCount = value; }
        }




        /// <summary>

        public int RemotePort
        {
            get { return m_remotePort; }
            set
            {
                if (value < IPEndPoint.MinPort || value > IPEndPoint.MaxPort)
                {
                    throw log4net.Util.SystemInfo.CreateArgumentOutOfRangeException("value", (object)value,
                        "The value specified is less than " +
                        IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo) +
                        " or greater than " +
                        IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo) + ".");
                }
                else
                {
                    m_remotePort = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets <see cref="Encoding"/> used to write the packets.
        /// </summary>
        /// <value>
        /// The <see cref="Encoding"/> used to write the packets.
        /// </value>
        /// <remarks>
        /// <para>
        /// The <see cref="Encoding"/> used to write the packets.
        /// </para>
        /// </remarks>
        public Encoding Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }

        #endregion Public Instance Properties

        #region Protected Instance Properties


        protected TcpClient Client
        {
            get { return this.m_client; }
            set { this.m_client = value; }
        }

        /// <summary>
        /// Gets or sets the cached remote endpoint to which the logging events should be sent.
        /// </summary>
        /// <value>
        /// The cached remote endpoint to which the logging events will be sent.
        /// </value>
        /// <remarks>
        /// The <see cref="ActivateOptions" /> method will initialize the remote endpoint 
        /// with the values of the <see cref="RemoteAddress" /> and <see cref="RemotePort"/>
        /// properties.
        /// </remarks>
        protected IPEndPoint RemoteEndPoint
        {
            get { return this.m_remoteEndPoint; }
            set { this.m_remoteEndPoint = value; }
        }

        #endregion Protected Instance Properties

        #region Implementation of IOptionHandler

        /// <summary>
        /// Initialize the appender based on the options set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// <para>
        /// The appender will be ignored if no <see cref="RemoteAddress" /> was specified or 
        /// an invalid remote or local TCP port number was specified.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">The required property <see cref="RemoteAddress" /> was not specified.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The TCP port number assigned to <see cref="LocalPort" /> or <see cref="RemotePort" /> is less than <see cref="IPEndPoint.MinPort" /> or greater than <see cref="IPEndPoint.MaxPort" />.</exception>
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            if (this.RemoteAddress == null)
            {
                throw new ArgumentNullException("The required property 'Address' was not specified.");
            }
            else if (this.RemotePort < IPEndPoint.MinPort || this.RemotePort > IPEndPoint.MaxPort)
            {
                throw log4net.Util.SystemInfo.CreateArgumentOutOfRangeException("this.RemotePort", (object)this.RemotePort,
                    "The RemotePort is less than " +
                    IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo) +
                    " or greater than " +
                    IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo) + ".");
            }
            else
            {
                this.RemoteEndPoint = new IPEndPoint(this.RemoteAddress, this.RemotePort);
                this.InitializeClientConnection();
            }

            thQueue = new Thread(this.InnerEnqueueProcessor);
            thQueue.Start();

        }

        Thread thQueue;

        #endregion

        #region Override implementation of AppenderSkeleton

        public readonly object dequeueLocker = new object();
        private ConcurrentQueue<string> senderLocalQueue = new ConcurrentQueue<string>();

        ManualResetEvent InnerEnqueueProcessor_MRE = new ManualResetEvent(false);
        public void InnerEnqueueProcessor()
        {
            while (true)
            {
                try
                {
                    InnerEnqueueProcessor_MRE.WaitOne();
                    lock (dequeueLocker)
                    {
                        if (senderLocalQueue.Count > 0)
                        {
                            string value;
                            senderLocalQueue.TryPeek(out value);

                            bool sentOk = false;
                            if (value != null)
                            {
                                sentOk = SendInner(value);
                            }
                            else
                            {
                                Trace.WriteLine("null from queue. (InnerEnqueueProcessor)");
                            }
                            if (sentOk)
                            {
                                senderLocalQueue.TryDequeue(out value);
                            }
                            else
                            {
                                // if remain count>=10000 and have occured a tcp error just a moment ago, dequeue a message to save memory.
                                while (senderLocalQueue.Count >= this.MaxQueueItemCount)
                                {
                                    senderLocalQueue.TryDequeue(out value);
                                }
                            }
                        }
                        else
                        {
                            InnerEnqueueProcessor_MRE.Reset();
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                    break;
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(
                       "Error",
                       ex,
                       ErrorCode.WriteFailure);
                    Thread.Sleep(1000);
                }
            }
        }

        private bool SendInner(string logRenderStrng)
        {
            try
            {
                lock (this.Client)
                {
                    if (!this.Client.Connected)
                    {
                        InitializeClientConnection();
                        this.Client.Connect(this.RemoteEndPoint);
                    }

                    Byte[] buffer = m_encoding.GetBytes(logRenderStrng.ToCharArray());
                    this.Client.Client.Send(buffer);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(
                    "Unable to send logging event to remote host " +
                    this.RemoteAddress.ToString() +
                    " on port " +
                    this.RemotePort + ".",
                    ex,
                    ErrorCode.WriteFailure);
                return false;
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent != null)
            {
                //Must render message in the original thread, otherwise, loggingEvent's threadName will change to new thread.
                var logRenderStrng = RenderLoggingEvent(loggingEvent);
                //ConcurrentQueue is thread safe. 
                // append no wait.
                senderLocalQueue.Enqueue(logRenderStrng);
                InnerEnqueueProcessor_MRE.Set();
            }
        }

        /// <summary>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </summary>
        /// <value><c>true</c></value>
        /// <remarks>
        /// <para>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </para>
        /// </remarks>
        override protected bool RequiresLayout
        {
            get { return true; }
        }


        override protected void OnClose()
        {
            base.OnClose();

            if (this.Client != null)
            {
                this.Client.Close();
                this.Client = null;
            }

            thQueue?.Abort();

            string temp;
            while (senderLocalQueue.TryDequeue(out temp))
            {

            }
        }

        #endregion Override implementation of AppenderSkeleton

        #region Protected Instance Methods

        protected virtual void InitializeClientConnection()
        {
            try
            {
                this.Client = new TcpClient(RemoteAddress.AddressFamily);
                this.Client.SendTimeout = 5 * 1000;
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(
                    "Could not initialize the TcpClient connection.",
                    ex,
                    ErrorCode.GenericFailure);

                this.Client = null;
            }
        }

        #endregion Protected Instance Methods

        #region Private Instance Fields

        /// <summary>
        /// The IP address of the remote host or multicast group to which 
        /// the logging event will be sent.
        /// </summary>
        private IPAddress m_remoteAddress;

        /// <summary>
        /// The TCP port number of the remote host or multicast group to 
        /// which the logging event will be sent.
        /// </summary>
        private int m_remotePort;

        /// <summary>
        /// The cached remote endpoint to which the logging events will be sent.
        /// </summary>
        private IPEndPoint m_remoteEndPoint;


        private TcpClient m_client;

        /// <summary>
        /// The encoding to use for the packet.
        /// </summary>

        private Encoding m_encoding = Encoding.UTF8;


        #endregion Private Instance Fields
    }
}