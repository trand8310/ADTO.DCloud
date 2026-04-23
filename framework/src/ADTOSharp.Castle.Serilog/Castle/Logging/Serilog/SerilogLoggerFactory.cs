using System;
using Castle.Core.Logging;
using Serilog;
using ILogger = Castle.Core.Logging.ILogger;
using ISerilogLogger = Serilog.ILogger;

namespace ADTOSharp.Castle.Logging.Serilog;

public class SerilogLoggerFactory : AbstractLoggerFactory
{
    private readonly ISerilogLogger _logger;

    public SerilogLoggerFactory() : this(Log.Logger)
    {
    }
    public SerilogLoggerFactory(ISerilogLogger logger)
    {
        this._logger = logger;
    }

    public override ILogger Create(string name)
    {
        return new SerilogLogger(this._logger.ForContext("SourceContext", name, false), this);
    }

    public override ILogger Create(string name, LoggerLevel level)
    {
        throw new NotSupportedException("Logger levels cannot be set at runtime. Please see Serilog's LoggerConfiguration.MinimumLevel.");
    }
}