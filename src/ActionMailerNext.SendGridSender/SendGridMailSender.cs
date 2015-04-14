using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActionMailerNext.SendGridSender
{
    using System.Configuration;
    using System.IO;
    using System.Net;
    using ActionMailerNext.Interfaces;
    using SendGrid;
    using System.Linq;
    using System.Net.Mime;
    using ActionMailerNext.Utils;

    public class SendGridMailSender : IMailSender
    {
        private readonly Web _client;

        public SendGridMailSender()
        {
            var username = ConfigurationManager.AppSettings["SendGridUser"];
            var password = ConfigurationManager.AppSettings["SendGridPass"];
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(
                    "The AppSetting 'SendGridUser' and 'SendGridPass' are not defined correctly. Either define this configuration section or use the constructor with username and password parameter.");

            var credentials = new NetworkCredential(username, password);
            _client = new Web(credentials);
        }

        public SendGridMailSender(string username, string password)
        {
            var credentials = new NetworkCredential(username, password);
            _client = new Web(credentials);
        }

        /// <summary>
        ///     Creates a MailMessage for the current MailAttribute instance.
        /// </summary>
        protected SendGridMessage GenerateProspectiveMailMessage(MailAttributes mail)
        {
            // Basic message attributes
            var message = new SendGridMessage
            {
                From = mail.From,
                To = mail.To.ToArray(),
                Cc = mail.Cc.ToArray(),
                Bcc = mail.Bcc.ToArray(),
                ReplyTo = mail.ReplyTo.ToArray(),
                Subject = mail.Subject,
                Headers = (Dictionary<string, string>)mail.Headers
            };

            // Message content
            foreach (var view in mail.AlternateViews)
            {
                using (var reader = new StreamReader(view.ContentStream))
                {
                    var body = reader.ReadToEnd();

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Plain)
                    {
                        message.Text = body;
                    }
                    if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                    {
                        message.Html = body;
                    }
                }
            }

            // Attachments
            foreach (var mailAttachment in mail.Attachments.Select(attachment => AttachmentCollection.ModifyAttachmentProperties(attachment.Key,
                attachment.Value,
                false)))
            {
                using (var stream = new MemoryStream())
                {
                    mailAttachment.ContentStream.CopyTo(stream);
                    message.AddAttachment(stream, mailAttachment.Name);
                }
            }

            return message;
        }

        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            _client.Deliver(mail);

            return null;
        }

        public virtual async Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            await _client.DeliverAsync(mail);

            return null;
        }

        public void Dispose()
        {
        }
    }
}
