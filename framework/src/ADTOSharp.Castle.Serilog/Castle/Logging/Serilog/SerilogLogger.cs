using System;
using ILogger = Castle.Core.Logging.ILogger;
using ISerilogLogger = Serilog.ILogger;
using LogEventLevel = Serilog.Events.LogEventLevel;

namespace ADTOSharp.Castle.Logging.Serilog;

[Serializable]
public class SerilogLogger :MarshalByRefObject,ILogger
{
    private static readonly Type DeclaringType = typeof(SerilogLogger);
    protected internal SerilogLoggerFactory Factory { get; set; }
    protected internal ISerilogLogger Logger { get; set; }

    public SerilogLogger(ISerilogLogger logger, SerilogLoggerFactory factory)
    {
        Logger = logger;
        Factory = factory;
    }

    public SerilogLogger()
    {

    }

    public bool IsTraceEnabled
    {
        get { return Logger.IsEnabled(LogEventLevel.Verbose); }
    }
    public bool IsDebugEnabled
    {
        get { return Logger.IsEnabled(LogEventLevel.Debug); }
    }

    public bool IsErrorEnabled
    {
        get { return Logger.IsEnabled(LogEventLevel.Error); }
    }

    public bool IsFatalEnabled
    {
        get { return Logger.IsEnabled(LogEventLevel.Fatal); }
    }

    public bool IsInfoEnabled
    {
        get { return Logger.IsEnabled(LogEventLevel.Information); }
    }

    public bool IsWarnEnabled
    {
        get { return Logger.IsEnabled(LogEventLevel.Warning); }
    }


    public override string ToString()
    {
        return Logger.ToString();
    }

    public virtual ILogger CreateChildLogger(string name)
    {
        throw new NotImplementedException("Creating child loggers for Serilog is not supported");
    }

    public void Trace(string message)
    {
        if (IsTraceEnabled)
        {
            Logger.Verbose(message);
        }
    }

    public void Trace(Func<string> messageFactory)
    {
        if (IsTraceEnabled)
        {
            Logger.Verbose(messageFactory.Invoke());
        }
    }

    public void Trace(string message, Exception exception)
    {
        Logger.Verbose(exception, message);
    }

    public void TraceFormat(string format, params object[] args)
    {
        if (IsTraceEnabled)
        {
            Logger.Verbose(format, args);
        }
    }

    public void TraceFormat(Exception exception, string format, params object[] args)
    {
        if (IsTraceEnabled)
        {
            Logger.Verbose(exception, format, args);
        }
    }

    public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsTraceEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Verbose(string.Format(formatProvider, format, args));
        }
    }

    public void TraceFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsTraceEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Verbose(exception, string.Format(formatProvider, format, args));
        }
    }

    public void Debug(string message)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(message);
        }
    }

    public void Debug(Func<string> messageFactory)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(messageFactory.Invoke());
        }
    }

    public void Debug(string message, Exception exception)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(exception, message);
        }
    }

    public void DebugFormat(string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(format, args);
        }
    }

    public void DebugFormat(Exception exception, string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            Logger.Debug(exception, format, args);
        }
    }

    public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Debug(string.Format(formatProvider, format, args));
        }
    }

    public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Debug(exception, string.Format(formatProvider, format, args));
        }
    }

    public void Error(string message)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(message);
        }
    }

    public void Error(Func<string> messageFactory)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(messageFactory.Invoke());
        }
    }

    public void Error(string message, Exception exception)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(exception, message);
        }
    }

    public void ErrorFormat(string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(format, args);
        }
    }

    public void ErrorFormat(Exception exception, string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            Logger.Error(exception, format, args);
        }
    }

    public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Error(string.Format(formatProvider, format, args));
        }
    }

    public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Error(exception, string.Format(formatProvider, format, args));
        }
    }

    public void Fatal(string message)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(message);
        }
    }

    public void Fatal(Func<string> messageFactory)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(messageFactory.Invoke());
        }
    }

    public void Fatal(string message, Exception exception)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(exception, message);
        }
    }

    public void FatalFormat(string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(format, args);
        }
    }

    public void FatalFormat(Exception exception, string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            Logger.Fatal(exception, format, args);
        }
    }

    public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Fatal(string.Format(formatProvider, format, args));
        }
    }

    public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Fatal(exception, string.Format(formatProvider, format, args));
        }
    }

    public void Info(string message)
    {
        if (IsInfoEnabled)
        {
            Logger.Information(message);
        }
    }

    public void Info(Func<string> messageFactory)
    {
        if (IsInfoEnabled)
        {
            Logger.Information(messageFactory.Invoke());
        }
    }

    public void Info(string message, Exception exception)
    {
        if (IsInfoEnabled)
        {
            Logger.Information(exception, message);
        }
    }

    public void InfoFormat(string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            Logger.Information(format, args);
        }
    }

    public void InfoFormat(Exception exception, string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            Logger.Information(exception, format, args);
        }
    }

    public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Information(string.Format(formatProvider, format, args));
        }
    }

    public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Information(exception, string.Format(formatProvider, format, args));
        }
    }

    public void Warn(string message)
    {
        if (IsWarnEnabled)
        {
            Logger.Warning(message);
        }
    }

    public void Warn(Func<string> messageFactory)
    {
        if (IsWarnEnabled)
        {
            Logger.Warning(messageFactory.Invoke());
        }
    }

    public void Warn(string message, Exception exception)
    {
        if (IsWarnEnabled)
        {
            Logger.Warning(exception, message);
        }
    }

    public void WarnFormat(string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            Logger.Warning(format, args);
        }
    }

    public void WarnFormat(Exception exception, string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            Logger.Warning(exception, format, args);
        }
    }

    public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Warning(string.Format(formatProvider, format, args));
        }
    }

    public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            //TODO: This honours the formatProvider rather than passing through args for structured logging
            Logger.Warning(exception, string.Format(formatProvider, format, args));
        }
    }
}