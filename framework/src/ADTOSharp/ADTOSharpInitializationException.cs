using System;
using System.Runtime.Serialization;

namespace ADTOSharp
{
    /// <summary>
    /// This exception is thrown if a problem on ADTO initialization progress.
    /// </summary>
    [Serializable]
    public class ADTOSharpInitializationException : ADTOSharpException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ADTOSharpInitializationException()
        {

        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public ADTOSharpInitializationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public ADTOSharpInitializationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ADTOSharpInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
