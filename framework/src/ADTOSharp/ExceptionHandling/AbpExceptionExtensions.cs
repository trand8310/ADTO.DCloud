using ADTOSharp.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.ExceptionServices;


namespace ADTOSharp.ExceptionHandling;

/// <summary>
/// Extension methods for <see cref="Exception"/> class.
/// </summary>
public static class AbpExceptionExtensions
{
    /// <summary>
    /// Uses <see cref="ExceptionDispatchInfo.Capture"/> method to re-throws exception
    /// while preserving stack trace.
    /// </summary>
    /// <param name="exception">Exception to be re-thrown</param>
    public static void ReThrow(this Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }

    /// <summary>
    /// Try to get a log level from the given <paramref name="exception"/>
    /// if it implements the <see cref="IHasLogSeverity"/> interface.
    /// Otherwise, returns the <paramref name="defaultLevel"/>.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="defaultLevel"></param>
    /// <returns></returns>
    public static LogSeverity GetLogLevel(this Exception exception, LogSeverity defaultLevel = LogSeverity.Error)
    {
        return (exception as IHasLogSeverity)?.Severity ?? defaultLevel;
    }
}
