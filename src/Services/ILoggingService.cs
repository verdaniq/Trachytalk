using System;
using Sentry;

namespace Trachytalk.Services;

public interface ILoggingService
{
    void LogError(Exception exception);
    void LogMessage(string message);
}

public class LoggingService : ILoggingService
{
    public void LogError(Exception exception)
    {
        LogMessage(exception.Message);
        LogMessage(exception.StackTrace);
        SentrySdk.CaptureException(exception);
    }
    
    public void LogMessage(string message)
    {
        SentrySdk.CaptureMessage(message);
    }
}