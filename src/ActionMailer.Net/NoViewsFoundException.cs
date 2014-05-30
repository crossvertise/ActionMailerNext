using System;

namespace ActionMailer.Net {
    /// <summary>
    /// Thrown when ActionMailer cannot locate any views for a given EmailResult
    /// </summary>
    public class NoViewsFoundException : Exception {
        /// <summary>
        /// Thrown when ActionMailer cannot locate any views for a given EmailResult
        /// </summary>
        public NoViewsFoundException() { }

        /// <summary>
        /// Thrown when ActionMailer cannot locate any views for a given EmailResult
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        public NoViewsFoundException(string message) : base(message) { }

        /// <summary>
        /// Thrown when ActionMailer cannot locate any views for a given EmailResult
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">An inner exception which contributed to (or caused) this exception.</param>
        public NoViewsFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
