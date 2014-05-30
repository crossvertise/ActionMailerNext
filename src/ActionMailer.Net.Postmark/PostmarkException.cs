using System;

namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// An exception that wraps the PostmarkResponse object.
    /// </summary>
    public class PostmarkException : Exception {
        /// <summary>
        /// The actual response received from the Postmark API.
        /// </summary>
        public PostmarkResponse PostmarkResponse { get; set; }

        /// <summary>
        /// Creates a new PostmarkException that wraps the given response.
        /// </summary>
        /// <param name="response">The response received from Postmark.</param>
        public PostmarkException(PostmarkResponse response) : base(response.Message) {
            PostmarkResponse = response;
        }
    }
}
