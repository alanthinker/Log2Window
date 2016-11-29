
namespace Log2Window.Log
{
    /// <summary>
    /// This interface must be implemented to be notified when new log message arrives.
    /// </summary>
    public interface ILogMessageNotifiable
	{ 
        /// <summary>
        /// Call this method when a new log message is arrived.
        /// </summary>
        /// <param name="logMsg">The message to log.</param>
        void Notify(LogMessage logMsg);
    }
}
