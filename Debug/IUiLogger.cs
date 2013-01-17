using System;

namespace FrbUi.Debug
{
    public interface IUiLogger : IDisposable
    {
        void LogMessage(ILayoutable sender, string messageType, string message);
    }
}
