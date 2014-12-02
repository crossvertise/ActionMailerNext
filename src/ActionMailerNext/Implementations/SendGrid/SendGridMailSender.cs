using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Utils;
using SendGrid;

namespace ActionMailerNext.Implementations.SendGrid
{
    /// <summary>
    ///     Implements IMailSender by using SendGrid.Web
    /// </summary>
    public class SendGridMailSender : IMailSender
    {
        private readonly Web _client;

        /// <summary>
        ///     Creates a new SendGridMailSender based on SendGrid.Web
        /// </summary>
        public SendGridMailSender()
        {
            var username = ConfigurationManager.AppSettings["SendGridUser"];
            var password = ConfigurationManager.AppSettings["SendGridPass"];
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(
                    "The AppSetting 'SendGridUser' and 'SendGridPass'  are not defined correctly. Either define this configuration section or use the constructor with username and password parameter.");

            var credentials = new NetworkCredential(username, password);
            _client = new Web(credentials);
        }

        /// <summary>
        ///     Creates a new SendGridMailMessage sender based on SendGridApi
        /// </summary>
        public SendGridMailSender(string username, string password)
        {
            var credentials = new NetworkCredential(username, password);
            _client = new Web(credentials);
        }
        
        /// <summary>
        ///     Creates a SendGridMessage for the current SendGridMailAttributes instance.
        /// </summary>
        protected SendGridMessage GenerateProspectiveMailMessage(MailAttributes mail)
        {

            if (mail.Cc.Any())
                throw new NotSupportedException("The CC field is not supported with the SendGridMailSender");

            if (mail.Bcc.Any())
                throw new NotSupportedException("The ReplyTo field is not supported with the SendGridMailSender");

            //create base message
            var message = new SendGridMessage
            {
                From = mail.From,
                To = mail.To.ToArray(),
                ReplyTo = mail.ReplyTo.ToArray(),
                Subject = mail.Subject,
                Headers = (Dictionary<string, string>)mail.Headers
            };

            //add content
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

            //add attachments
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


        /// <summary>
        ///     Sends SendGridMailMessage synchronously.
        /// </summary>
        /// <param name="mailAttributes">The SendGridMailAttributes you wish to send.</param>
        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            _client.Deliver(mail);

            return null;
        }

        /// <summary>
        ///     Sends SendGridMailMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The MailAttributes message you wish to send.</param>
        public virtual async Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            await _client.DeliverAsync(mail);

            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            
        }
    }
}