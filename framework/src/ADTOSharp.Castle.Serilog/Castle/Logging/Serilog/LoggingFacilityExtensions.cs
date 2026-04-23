using Castle.Facilities.Logging;
using Serilog;
using System;


namespace ADTOSharp.Castle.Logging.Serilog;

public static class LoggingFacilityExtensions
{
    //public static LoggingFacility UseSerilog(this LoggingFacility loggingFacility, IConfiguration configuration)
    //{
    //    //Logger logger = ConfigurationLoggerConfigurationExtensions.Configuration(loggingFacility, configuration);
    //    Logger logger = ConfigurationloggerConfigurationxtensions.Configuration(new LoggerConfiguration().ReadFrom, configuration).CreateLogger();
    //    return loggingFacility.LogUsing<SerilogLoggerFactory>(new SerilogLoggerFactory(logger));
    //}

    public static LoggingFacility UseADTOSharpSerilog(this LoggingFacility loggingFacility, Action<LoggerConfiguration> configureLogger)
    {
        if (loggingFacility == null)
        {
            throw new ArgumentNullException(nameof(LoggingFacility));
        }
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
        if(configureLogger != null)
        {
            configureLogger(loggerConfiguration);
        }
        ILogger logger = loggerConfiguration.CreateLogger();
       
        loggingFacility = loggingFacility.LogUsing<SerilogLoggerFactory>(new SerilogLoggerFactory(logger));
        return loggingFacility;
    }
}