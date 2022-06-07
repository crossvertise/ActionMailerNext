using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;
using System.Configuration;
using ActionMailerNext.Interfaces;

namespace ActionMailerNext.MandrillMailSender
{
    using sib_api_v3_sdk.Api;
    using sib_api_v3_sdk.Client;
    using sib_api_v3_sdk.Model;
    using System.Globalization;
    using System.Text;
    using System.Threading;

    public class SendInBlueMailSender : IMailSender
    {
        private readonly IMailInterceptor interceptor;
        private readonly TransactionalEmailsApi client;

        public SendInBlueMailSender() : this(ConfigurationManager.AppSettings["SendInBlueApiKey"], null) { }

        public SendInBlueMailSender(string apiKey, IMailInterceptor interceptor)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey",
                    "The AppSetting 'SendInBlueApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            this.interceptor = interceptor;
            Configuration.Default.ApiKey.Add("api-key", apiKey);
            this.client = new TransactionalEmailsApi();
        }

        /// <summary>
        ///     Creates a MailMessage for the current MailAttribute instance.
        /// </summary>
        protected SendSmtpEmail GenerateProspectiveMailMessage(MailAttributes mail)
        {
            var idnmapping = new IdnMapping();

            var emailAddresses = mail.To
                .Select(
                    t =>
                    {
                        var domainSplit = t.Address.Split('@');
                        return new SendSmtpEmailTo(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1]));
                    })
                .Union(
                    mail.Cc.Select(
                        t =>
                        {
                            var domainSplit = t.Address.Split('@');
                            return new SendSmtpEmailTo(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1]));
                        }))
                .Union(
                    mail.Bcc.Select(
                        t =>
                        {
                            var domainSplit = t.Address.Split('@');
                            return new SendSmtpEmailTo(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1]));
                        })).ToList();

            var emailHeaders = new Dictionary<string, string>();
            //create base message
            var message = new SendSmtpEmail
            {
                Sender = new SendSmtpEmailSender(mail.From.DisplayName, mail.From.Address),
                To = emailAddresses,
                Subject = mail.Subject
            };

            // We need to set Reply-To as a custom header
            if (mail.ReplyTo.Any())
            {
                emailHeaders.Add("Reply-To", string.Join(" , ", mail.ReplyTo));
            }

            // Adding content to the message
            foreach (var view in mail.AlternateViews)
            {
                var reader = new StreamReader(view.ContentStream, Encoding.UTF8, true, 1024, true);

                var body = reader.ReadToEnd();

                if (view.ContentType.MediaType == MediaTypeNames.Text.Plain)
                {
                    message.TextContent = body;
                }
                if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                {
                    message.HtmlContent = body;
                }
            }

            // Going through headers and adding them to the message
            mail.Headers.ToList().ForEach(h => emailHeaders.Add(h.Key, h.Value));

            // Adding the attachments
            var attachments = new List<SendSmtpEmailAttachment>();
            foreach (var mailAttachment in mail.Attachments.Select(attachment => Utils.AttachmentCollection.ModifyAttachmentProperties(attachment.Key, attachment.Value, false)))
            {
                using (var stream = new MemoryStream())
                {
                    mailAttachment.ContentStream.CopyTo(stream);
                    var byteData = stream.ToArray();
                    attachments.Add(new SendSmtpEmailAttachment()
                    {
                        Content = byteData,
                        Name = ReplaceGermanCharacters(mailAttachment.Name)
                    });
                }
            }

            message.Attachment = attachments;
            message.Headers = emailHeaders;
            return message;
        }

        #region Send methods

        public virtual List<IMailResponse> Deliver(IEmailResult emailResult)
        {
            return this.Send(emailResult.MailAttributes);
        }

        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
           throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        private void AsyncSendCompleted(MailAttributes mail)
        {
            this.interceptor.OnMailSent(mail);
        }

        private string ReplaceGermanCharacters(string s)
        {
            return s.Replace("ö", "oe")
                    .Replace("ü", "ue")
                    .Replace("ä", "ae")
                    .Replace("Ö", "Oe")
                    .Replace("Ü", "Ue")
                    .Replace("Ä", "Ae")
                    .Replace("ß", "ss");
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
