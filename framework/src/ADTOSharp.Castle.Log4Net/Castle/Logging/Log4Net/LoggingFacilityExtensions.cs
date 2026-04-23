using Castle.Facilities.Logging;

namespace ADTOSharp.Castle.Logging.Log4Net
{
    public static class LoggingFacilityExtensions
    {
        public static LoggingFacility UseADTOSharpLog4Net(this LoggingFacility loggingFacility)
        {
            return loggingFacility.LogUsing<Log4NetLoggerFactory>();
        }
    }
}