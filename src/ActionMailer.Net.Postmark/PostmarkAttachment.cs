namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// Represents a mail attachment for messages sent via Postmark.
    /// </summary>
    public class PostmarkAttachment {
        /// <summary>
        /// The file name for the attachment.  This also serves as the content ID.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Base64-encoded string that represents the actual content of the attachment.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The MIME type for the attachment.
        /// </summary>
        public string ContentType { get; set; }
    }
}
