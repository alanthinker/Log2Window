using System;
using System.ComponentModel;

using Log2Window.Log;
using System.Text;
using System.Windows.Forms;

namespace Log2Window.Receiver
{
    [Serializable]
    public abstract class BaseReceiver : MarshalByRefObject, IReceiver, ICloneable
    {
        [NonSerialized]
        protected ILogMessageNotifiable Notifiable;

        [NonSerialized]
        private string _displayName;


        #region IReceiver Members

        public abstract string SampleClientConfig { get; }

        [Browsable(false)]
        public string DisplayName
        {
            get { return _displayName; }
            protected set { _displayName = value; }
        }

        string m_TextEncoding = "utf-8";

        [Category("Configuration")]
        [DisplayName("Encoding")]
        public virtual string TextEncoding
        {
            get { return m_TextEncoding; }
            set
            {
                try
                {
                    var tempEncoding= Encoding.GetEncoding(value);
                    this.m_TextEncoding = value;
                    this.encodingObject = tempEncoding;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }
        }

        private Encoding encodingObject=Encoding.UTF8;
        protected Encoding EncodingObject
        {
            get
            { 
                return encodingObject;
            }
        } 


        public abstract void Initialize();
        public abstract void Terminate();

        public virtual void Attach(ILogMessageNotifiable notifiable)
        {
            Notifiable = notifiable;
        }

        public virtual void Detach()
        { 
            Notifiable = null;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
