#region License
/* Copyright (C) 2012 by Scott W. Anderson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace ActionMailer.Net {
    /// <summary>
    /// Some simple extension methods for the IMailerBase interface so we can use
    /// them in all implementations.
    /// </summary>
    public static class MailerBaseExtensions {
        /// <summary>
        /// Creates a MailMessage for the given IMailerBase instance.
        /// </summary>
        /// <param name="mailer">The IMailerBase to generate the message for</param>
        public static MailMessage GenerateMail(this IMailerBase mailer) {
            var message = new MailMessage();

            for (var i = 0; i < mailer.To.Count(); i++)
                message.To.Add(mailer.To[i]);

            for (var i = 0; i < mailer.CC.Count(); i++)
                message.CC.Add(mailer.CC[i]);

            for (var i = 0; i < mailer.BCC.Count(); i++)
                message.Bcc.Add(mailer.BCC[i]);

            for (var i = 0; i < mailer.ReplyTo.Count(); i++)
                message.ReplyToList.Add(mailer.ReplyTo[i]);

            // From is optional because it could be set in <mailSettings>
            if (!String.IsNullOrWhiteSpace(mailer.From))
                message.From = new MailAddress(mailer.From);

            message.Subject = mailer.Subject;

            foreach (var kvp in mailer.Headers)
                message.Headers[kvp.Key] = kvp.Value;

            foreach (var kvp in mailer.Attachments)
                message.Attachments.Add(CreateAttachment(kvp.Key, kvp.Value, false));

            foreach (var kvp in mailer.Attachments.Inline)
                message.Attachments.Add(CreateAttachment(kvp.Key, kvp.Value, true));

            return message;
        }

        private static Attachment CreateAttachment(string fileName, byte[] fileContents, bool inline) {
            // ideally we'd like to find the mime type for each attachment automatically
            // based on the file extension.
            string mimeType = null;
            var extension = fileName.Substring(fileName.LastIndexOf("."));
            if (!string.IsNullOrEmpty(extension))
                mimeType = MimeTypes.ResolveByExtension(extension);

            var stream = new MemoryStream(fileContents);
            var attachment = new Attachment(stream, fileName, mimeType);
            attachment.ContentDisposition.Inline = inline;
            attachment.ContentId = fileName;
            return attachment;
        }
    }
}
