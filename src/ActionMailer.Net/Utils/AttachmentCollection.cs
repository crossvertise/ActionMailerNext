using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace ActionMailer.Net.Utils
{
    /// <summary>
    ///     A collection of attachments.  This is basically a glorified Dictionary.
    /// </summary>
    public class AttachmentCollection : Dictionary<string, Attachment>
    {
        /// <summary>
        ///     Constructs an empty AttachmentCollection object.
        /// </summary>
        public AttachmentCollection()
        {
            Inline = new Dictionary<string, Attachment>();
        }

        /// <summary>
        ///     Any attachments added to this collection will be treated
        ///     as inline attachments within the mail message.
        /// </summary>
        public Dictionary<string, Attachment> Inline { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="attachment"></param>
        /// <param name="inline"></param>
        /// <returns></returns>
        public static Attachment ModifyAttachmentProperties(string fileName, Attachment attachment, bool inline)
        {
            // ideally we'd like to find the mime type for each attachment automatically
            // based on the file extension.
            string mimeType = null;

            if (inline)
            {
                mimeType = "multipart/related";
            }
            else
            {
                var extension = fileName.Substring(fileName.LastIndexOf("."));
                if (!string.IsNullOrEmpty(extension))
                    mimeType = MimeTypes.ResolveByExtension(extension);
            }

            var modifiedAttachment = new Attachment(attachment.ContentStream, fileName, mimeType);
            modifiedAttachment.ContentDisposition.Inline = inline;
            modifiedAttachment.ContentId = fileName;
            return modifiedAttachment;
        }
    }
}