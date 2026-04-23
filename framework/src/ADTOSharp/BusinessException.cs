using System;
using ADTOSharp.ExceptionHandling;
using ADTOSharp.Logging;
 
namespace ADTOSharp;

public class BusinessException : Exception,
    IBusinessException,
    IHasErrorCode,
    IHasErrorDetails,
    IHasLogSeverity
{
    public string? Code { get; set; }

    public string? Details { get; set; }

    public LogSeverity Severity { get; set; }

    public BusinessException(
        string? code = null,
        string? message = null,
        string? details = null,
        Exception? innerException = null,
        LogSeverity logLevel = LogSeverity.Warn)
        : base(message, innerException)
    {
        Code = code;//.IsNullOrWhiteSpace() ? 0 : Convert.ToInt32(code);
        Details = details;
        Severity = logLevel;
    }

    public BusinessException WithData(string name, object value)
    {
        Data[name] = value;
        return this;
    }
}
