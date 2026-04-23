using System;
using System.Runtime.Serialization;

namespace ADTOSharp.Domain.Uow
{
    [Serializable]
    public class ADTOSharpDbConcurrencyException : ADTOSharpException
    {
        /// <summary>
        /// Creates a new <see cref="ADTOSharpDbConcurrencyException"/> object.
        /// </summary>
        public ADTOSharpDbConcurrencyException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpException"/> object.
        /// </summary>
        public ADTOSharpDbConcurrencyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpDbConcurrencyException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public ADTOSharpDbConcurrencyException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpDbConcurrencyException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ADTOSharpDbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}