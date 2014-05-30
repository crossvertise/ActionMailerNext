using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;

namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// Some Postmark-specific helper methods to use when sending messages.
    /// </summary>
    public static class MailMessageExtensions {
        /// <summary>
        /// Converts a MailMessage to it's PostmarkMessage equivalent.
        /// </summary>
        /// <param name="mail">The MailMessage to convert.</param>
        /// <returns>A PostmarkMessage that can be used to send the mail through Postmark.</returns>
        public static PostmarkMessage ToPostmarkMessage(this MailMessage mail) {
            var pmMail = new PostmarkMessage {
                From = mail.From.ToString(),
                To = String.Join(",", mail.To.Select(x => x.ToString())),
                Cc = mail.CC.Count > 0 ? String.Join(",", mail.CC.Select(x => x.ToString())) : null,
                Bcc = mail.Bcc.Count > 0 ? String.Join(",", mail.Bcc.Select(x => x.ToString())) : null,
                Subject = mail.Subject,
                ReplyTo = mail.ReplyToList.Count > 0 ? String.Join(",", mail.ReplyToList.Select(x => x.ToString())) : null
            };

            for (int i = 0; i < mail.Headers.Count; i++) {
                pmMail.Headers.Add(new PostmarkHeader {
                    Name = mail.Headers.Keys[i],
                    Value = mail.Headers[i]
                });
            }

            foreach (var view in mail.AlternateViews) {
                using (var reader = new StreamReader(view.ContentStream)) {
                    var body = reader.ReadToEnd();

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Plain)
                        pmMail.TextBody = body;

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                        pmMail.HtmlBody = body;
                }
            }

            foreach (var attachment in mail.Attachments) {
                using (var stream = new MemoryStream()) {
                    attachment.ContentStream.CopyTo(stream);
                    var base64Data = Convert.ToBase64String(stream.ToArray());

                    pmMail.Attachments.Add(new PostmarkAttachment {
                        Name = attachment.ContentId,
                        Content = base64Data,
                        ContentType = attachment.ContentType.MediaType
                    });
                }
            }

            return pmMail;
        }
    }
}
