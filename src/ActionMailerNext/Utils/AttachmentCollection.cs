namespace ActionMailerNext.Utils
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;

    /// <summary>
    /// A collection of attachments.  This is basically a glorified Dictionary.
    /// </summary>
    public class AttachmentCollection : Dictionary<string, byte[]>
    {
        public AttachmentCollection()
        {
            Inline = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// Any attachments added to this collection will be treated as inline attachments within the mail message.
        /// </summary>
        public Dictionary<string, byte[]> Inline { get; private set; }

        public static Attachment ModifyAttachmentProperties(string fileName, byte[] fileBytes, bool inline)
        {
            // Ideally we'd like to find the mime type for each attachment automatically based on the file extension.
            string mimeType = null;

            if (inline)
            {
                mimeType = "multipart/related";
            }
            else
            {
                var extension = fileName.Substring(fileName.LastIndexOf("."));
                if (!string.IsNullOrEmpty(extension))
                {
                    mimeType = MimeTypes.ResolveByExtension(extension);
                }
            }

            var memoryStream = new MemoryStream(fileBytes);
            var modifiedAttachment = new Attachment(memoryStream, fileName, mimeType);
            modifiedAttachment.ContentDisposition.Inline = inline;
            modifiedAttachment.ContentId = fileName;
            return modifiedAttachment;
        }
    }
}