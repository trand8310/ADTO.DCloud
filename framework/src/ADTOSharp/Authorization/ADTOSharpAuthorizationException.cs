using System;
using System.Runtime.Serialization;
using ADTOSharp.Logging;

namespace ADTOSharp.Authorization
{
    /// <summary>
    /// This exception is thrown on an unauthorized request.
    /// </summary>
    [Serializable]
    public class ADTOSharpAuthorizationException : ADTOSharpException, IHasLogSeverity
    {
        /// <summary>
        /// Default log severity
        /// </summary>
        public static LogSeverity DefaultLogSeverity = LogSeverity.Warn;
        
        /// <summary>
        /// Severity of the exception.
        /// Default: Warn.
        /// </summary>
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpAuthorizationException"/> object.
        /// </summary>
        public ADTOSharpAuthorizationException()
        {
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpAuthorizationException"/> object.
        /// </summary>
        public ADTOSharpAuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpAuthorizationException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public ADTOSharpAuthorizationException(string message)
            : base(message)
        {
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpAuthorizationException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ADTOSharpAuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Severity = DefaultLogSeverity;
        }
    }
}
