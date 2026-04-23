using System;

namespace ADTOSharp.Events.Bus.Exceptions
{
    /// <summary>
    /// This type of events are used to notify for exceptions handled by ADTO infrastructure.
    /// </summary>
    public class ADTOSharpHandledExceptionData : ExceptionData
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">Exception object</param>
        public ADTOSharpHandledExceptionData(Exception exception)
            : base(exception)
        {

        }
    }
}