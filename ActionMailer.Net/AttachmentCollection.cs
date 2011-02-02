using System.Collections.Generic;

namespace ActionMailer.Net {
    public class AttachmentCollection : Dictionary<string, byte[]> {
        public Dictionary<string, byte[]> Inline { get; private set; }
        public AttachmentCollection() {
            Inline = new Dictionary<string, byte[]>();
        }
    }
}
