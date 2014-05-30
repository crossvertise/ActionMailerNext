using System.Collections.Generic;

namespace ActionMailer.Net {
    /// <summary>
    /// A collection of attachments.  This is basically a glorified Dictionary.
    /// </summary>
    public class AttachmentCollection : Dictionary<string, byte[]> {
        /// <summary>
        /// Any attachments added to this collection will be treated
        /// as inline attachments within the mail message.
        /// </summary>
        public Dictionary<string, byte[]> Inline { get; private set; }

        /// <summary>
        /// Constructs an empty AttachmentCollection object.
        /// </summary>
        public AttachmentCollection() {
            Inline = new Dictionary<string, byte[]>();
        }
    }
}
