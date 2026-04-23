using System.Collections.Generic;
using System.Net;

namespace ADTOSharp.AspNetCore.ExceptionHandling;

public class ADTOSharpExceptionHttpStatusCodeOptions
{
    public IDictionary<string, HttpStatusCode> ErrorCodeToHttpStatusCodeMappings { get; }

    public ADTOSharpExceptionHttpStatusCodeOptions()
    {
        ErrorCodeToHttpStatusCodeMappings = new Dictionary<string, HttpStatusCode>();
    }

    public void Map(string errorCode, HttpStatusCode httpStatusCode)
    {
        ErrorCodeToHttpStatusCodeMappings[errorCode] = httpStatusCode;
    }
}
