using System;

namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// Represents the response received back from Postmark.
    /// </summary>
    public class PostmarkResponse {
        /// <summary>
        /// The error code.  0 indicates no error.  Any other number is an error.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// The message received from Postmark.  Can contain details for errors.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The Postmark-generated ID for the message that was sent.
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// The date and time when the message was submitted to Postmark.
        /// </summary>
        public DateTime SubmittedAt { get; set; }

        /// <summary>
        /// The recepient of the message.
        /// </summary>
        public string To { get; set; }
    }
}
