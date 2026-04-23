using ADTOSharp.Collections.Extensions;
using ADTOSharp.ExceptionHandling;
using ADTOSharp.Extensions;
using ADTOSharp.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Logging;


namespace Microsoft.Extensions.Logging;

public static class AbpLoggerExtensions
{
    public static void LogWithLevel(this ILogger logger, LogSeverity logLevel, string message)
    {
        switch (logLevel)
        {
            case LogSeverity.Error:
                logger.LogError(message);
                break;
            case LogSeverity.Warn:
                logger.LogWarning(message);
                break;
            case LogSeverity.Info:
                logger.LogInformation(message);
                break;
            default: // LogLevel.Debug || LogLevel.None
                logger.LogDebug(message);
                break;
        }
    }

    public static void LogWithLevel(this ILogger logger, LogSeverity logLevel, string message, Exception exception)
    {
        switch (logLevel)
        {
            case LogSeverity.Error:
                logger.LogError(exception, message);
                break;
            case LogSeverity.Warn:
                logger.LogWarning(exception, message);
                break;
            case LogSeverity.Info:
                logger.LogInformation(exception, message);
                break;
            default: // LogLevel.Debug || LogLevel.None
                logger.LogDebug(exception, message);
                break;
        }
    }

    public static void LogException(this ILogger logger, Exception ex, LogSeverity? level = null)
    {
        var selectedLevel = level ?? ex.GetLogLevel();

        logger.LogWithLevel(selectedLevel, ex.Message, ex);
        LogKnownProperties(logger, ex, selectedLevel);
        LogSelfLogging(logger, ex);
        LogData(logger, ex, selectedLevel);
    }

    private static void LogKnownProperties(ILogger logger, Exception exception, LogSeverity logLevel)
    {
        if (exception is IHasErrorCode exceptionWithErrorCode && !exceptionWithErrorCode.Code.IsNullOrWhiteSpace())
        {
            logger.LogWithLevel(logLevel, "Code:" + exceptionWithErrorCode.Code);
        }

        if (exception is IHasErrorDetails exceptionWithErrorDetails && !exceptionWithErrorDetails.Details.IsNullOrWhiteSpace())
        {
            logger.LogWithLevel(logLevel, "Details:" + exceptionWithErrorDetails.Details);
        }
    }

    private static void LogData(ILogger logger, Exception exception, LogSeverity logLevel)
    {
        if (exception.Data.Count <= 0)
        {
            return;
        }

        var exceptionData = new StringBuilder();
        exceptionData.AppendLine("---------- Exception Data ----------");
        foreach (var key in exception.Data.Keys)
        {
            exceptionData.AppendLine($"{key} = {exception.Data[key]}");
        }

        logger.LogWithLevel(logLevel, exceptionData.ToString());
    }

    private static void LogSelfLogging(ILogger logger, Exception exception)
    {
        var loggingExceptions = new List<IExceptionWithSelfLogging>();

        if (exception is IExceptionWithSelfLogging logging)
        {
            loggingExceptions.Add(logging);
        }
        else if (exception is AggregateException aggException && aggException.InnerException != null)
        {
            if (aggException.InnerException is IExceptionWithSelfLogging selfLogging)
            {
                loggingExceptions.Add(selfLogging);
            }

            foreach (var innerException in aggException.InnerExceptions)
            {
                if (innerException is IExceptionWithSelfLogging withSelfLogging)
                {
                    loggingExceptions.AddIfNotContains(withSelfLogging);
                }
            }
        }

        foreach (var ex in loggingExceptions)
        {
            ex.Log(logger);
        }
    }
}
