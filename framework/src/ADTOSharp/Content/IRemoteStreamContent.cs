using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTOSharp.Content
{
    public interface IRemoteStreamContent : IDisposable
    {
        string? FileName { get; }

        string ContentType { get; }

        long? ContentLength { get; }

        Stream GetStream();
    }
}
