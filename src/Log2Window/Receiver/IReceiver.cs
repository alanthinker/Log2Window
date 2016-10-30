using Log2Window.Log;


namespace Log2Window.Receiver
{
    public interface IReceiver
    {
        string SampleClientConfig { get; }
        string DisplayName { get; }

        void Initialize();
        void Terminate();

        void Attach(ILogMessageNotifiable notifiable);
        void Detach();
    }
}
