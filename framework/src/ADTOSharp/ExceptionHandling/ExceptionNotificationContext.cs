using System;
 
using ADTOSharp.Extensions;
using ADTOSharp.Logging;
using JetBrains.Annotations;
 

namespace ADTOSharp.ExceptionHandling;

public class ExceptionNotificationContext
{
    /// <summary>
    /// The exception object.
    /// </summary>
    [NotNull]
    public Exception Exception { get; }

    public LogSeverity LogLevel { get; }
 
    /// <summary>
    /// True, if it is handled.
    /// </summary>
    public bool Handled { get; }

    public ExceptionNotificationContext(
        [NotNull] Exception exception,
        LogSeverity? logLevel = null,
        bool handled = true)
    {
        Exception = Check.NotNull(exception, nameof(exception));
        LogLevel = logLevel ?? exception.GetLogLevel();
        Handled = handled;
    }
}
