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
                    Encoding.GetEncoding(value);
                    m_TextEncoding = value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }
        }

        private Encoding encodingObject;
        protected Encoding EncodingObject
        {
            get
            {
                if (encodingObject == null)
                {
                    encodingObject = System.Text.Encoding.GetEncoding(m_TextEncoding);
                }
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
