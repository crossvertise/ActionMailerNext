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
    public class SendGridMailSender : IMailSender, IDisposable
    {
        private bool disposed = false;

        private IMailInterceptor _interceptor;
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

        public SendGridMailSender(string username, string password, IMailInterceptor interceptor)
        {
            _interceptor = interceptor;
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
                    stream.Seek(0, SeekOrigin.Begin);
                    mailAttachment.ContentStream.CopyTo(stream);
                    message.AddAttachment(stream, mailAttachment.Name);
                }
            }
            return message;
        }

        #region Send methods

        public virtual List<IMailResponse> Deliver(IEmailResult emailResult)
        {
            return this.Send(emailResult.MailAttributes);
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

        /// <summary>
        ///     Sends your message asynchronously.  This method does not block.  If you need to know
        ///     when the message has been sent, then override the OnMailSent method in MailerBase which
        ///     will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public async Task<MailAttributes> DeliverAsync(IEmailResult emailResult)
        {
            var deliverTask = this.SendAsync(emailResult.MailAttributes);
            await deliverTask.ContinueWith(t => AsyncSendCompleted(emailResult.MailAttributes));

            return emailResult.MailAttributes;
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

        #endregion

        #region Private methods

        private void AsyncSendCompleted(MailAttributes mail)
        {
            _interceptor.OnMailSent(mail);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing) { }

        #endregion
    }
}
