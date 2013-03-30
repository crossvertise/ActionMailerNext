using System;
using System.Collections.Generic;

namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// A simple POCO that represents a message to be sent via postmark.
    /// </summary>
    [Serializable]
    public class PostmarkMessage {
        /// <summary>
        /// Who the message is from.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// A comma-separated list of addresses to send the message to.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// A comma-separated list of addresses to carbon-copy the message to.
        /// </summary>
        public string Cc { get; set; }

        /// <summary>
        /// A comma-separated list of addresses to blind carbon-copy the message to.
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// The subject of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// A special tag that you can specify for the message within the Postmark system.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The HTML body of the message.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// The plain text body of the message.
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// A comma-separated list of addresses that should be used for replies.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// A list of custom headers to attach to the message.
        /// </summary>
        public List<PostmarkHeader> Headers { get; set; }

        /// <summary>
        /// A list of attachments for the message.
        /// </summary>
        public List<PostmarkAttachment> Attachments { get; set; }

        /// <summary>
        /// Creates a new PostmarkMessage to use when sending mail via Postmark.
        /// </summary>
        public PostmarkMessage() {
            Headers = new List<PostmarkHeader>();
            Attachments = new List<PostmarkAttachment>();
        }
    }
}
