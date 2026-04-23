using System;
using ADTOSharp.ExceptionHandling;
using Volo.Abp.ExceptionHandling;

namespace ADTOSharp.Http.Client;

public class ADTOSharpRemoteCallException : ADTOSharpException, IHasErrorCode, IHasErrorDetails, IHasHttpStatusCode
{
    public int HttpStatusCode { get; set; }

    public string? Code { get; set; }

    public string? Details { get; set; }

    public RemoteServiceErrorInfo? Error { get; set; }
 

    public ADTOSharpRemoteCallException()
    {

    }

    public ADTOSharpRemoteCallException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = Error?.Code;
        Details = Error?.Details;
    }

    public ADTOSharpRemoteCallException(RemoteServiceErrorInfo error, Exception? innerException = null)
        : base(error.Message, innerException)
    {
        Error = error;

        if (error.Data != null)
        {
            foreach (var dataKey in error.Data.Keys)
            {
                Data[dataKey] = error.Data[dataKey];
            }
        }
    }
}
