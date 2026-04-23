using System;
using System.Runtime.Serialization;

namespace ADTOSharp.BlobStoring
{
    public class BlobAlreadyExistsException : ADTOSharpException
    {
        public BlobAlreadyExistsException()
        {

        }

        public BlobAlreadyExistsException(string message)
            : base(message)
        {

        }

        public BlobAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public BlobAlreadyExistsException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}