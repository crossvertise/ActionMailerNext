using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;
using System.Configuration;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Mime;
using ActionMailerNext.Utils;
using SendGrid;

namespace ActionMailerNext.SendGridSender
{
    using System.Net.Configuration;
    using System.Runtime.InteropServices;

    public class SendGridMailSender : IMailSender, IDisposable
    {
        private bool disposed = false;

        private SafeHandle resource;
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
                var reader = new StreamReader(view.ContentStream);

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

            // Attachments
            foreach (
                var mailAttachment in
                    mail.Attachments.Select(
                        attachment =>
                        AttachmentCollection.ModifyAttachmentProperties(attachment.Key, attachment.Value, false)))
            {
                using (var stream = new MemoryStream())
                {
                    mailAttachment.ContentStream.CopyTo(stream);
                    mailAttachment.ContentStream.Seek(0, SeekOrigin.Begin);
                    message.AddAttachment(stream, mailAttachment.Name);
                }
            }

            return message;
        }

        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            var response = new List<IMailResponse>();

            _client.Deliver(mail);

            for (int i = 0; i < mailAttributes.To.Count; i++)
            {
                response.Add(new SendGridMailResponse
                {
                    Email = mailAttributes.To[i].Address,
                    Status = "E-mail delivered successfully."
                });
            }

            return response;
        }

        public virtual async Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            var response = new List<IMailResponse>();

            await _client.DeliverAsync(mail);

            for (int i = 0; i <= mailAttributes.To.Count; i++)
            {
                response.Add(new SendGridMailResponse
                {
                    Email = mailAttributes.To[i].Address,
                    Status = "E-mail delivered successfully."
                });
            }

            return response;
        }

        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (resource != null) resource.Dispose();
            }
        }
    }
}
