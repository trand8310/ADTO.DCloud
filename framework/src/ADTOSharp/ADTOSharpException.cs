using System;
using System.Runtime.Serialization;

namespace ADTOSharp
{
    /// <summary>
    /// Base exception type for those are thrown by ADTOSharp system for ADTOSharp specific exceptions.
    /// </summary>
    [Serializable]
    public class ADTOSharpException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="ADTOSharpException"/> object.
        /// </summary>
        public ADTOSharpException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpException"/> object.
        /// </summary>
        public ADTOSharpException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public ADTOSharpException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ADTOSharpException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
