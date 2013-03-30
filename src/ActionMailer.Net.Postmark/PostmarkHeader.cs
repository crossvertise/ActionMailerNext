namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// Represents a header used for mail messages sent through Postmark
    /// </summary>
    public class PostmarkHeader {
        /// <summary>
        /// The name of the header.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value for the header.
        /// </summary>
        public string Value { get; set; }
    }
}
