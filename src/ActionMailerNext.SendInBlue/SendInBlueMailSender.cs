

namespace ActionMailerNext.MandrillMailSender
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading.Tasks;

    using sib_api_v3_sdk.Api;
    using sib_api_v3_sdk.Model;
    using ActionMailerNext.Interfaces;

    public class SendInBlueMailSender : IMailSender
    {
        private readonly IMailInterceptor _interceptor;
        private readonly TransactionalEmailsApi _client;

        public SendInBlueMailSender() : this(ConfigurationManager.AppSettings["SendInBlueApiKey"], null) { }

        public SendInBlueMailSender(string apiKey, IMailInterceptor interceptor)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey",
                    "The AppSetting 'SendInBlueApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            _interceptor = interceptor;
            if (!sib_api_v3_sdk.Client.Configuration.Default.ApiKey.ContainsKey("api-key"))
            {
                sib_api_v3_sdk.Client.Configuration.Default.ApiKey.Add("api-key", apiKey);
            }
            _client = new TransactionalEmailsApi();
        }

        /// <summary>
        ///     Creates a MailMessage for the current MailAttribute instance.
        /// </summary>
        public SendSmtpEmail GenerateProspectiveMailMessage(MailAttributes mail)
        {
            var idnmapping = new IdnMapping();

            // Create base message
            var message = new SendSmtpEmail
            {
                Sender = new SendSmtpEmailSender(mail.From.DisplayName, mail.From.Address),
                To = mail.To.Select(
                    t =>
                    {
                        var domainSplit = t.Address.Split('@');
                        return new SendSmtpEmailTo(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1]));
                    }).ToList(),
                Bcc = mail.Bcc.Select(
                        t =>
                        {
                            var domainSplit = t.Address.Split('@');
                            return new SendSmtpEmailBcc(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1]));
                        }).ToList(),
                Cc = mail.Cc.Select(
                        t =>
                        {
                            var domainSplit = t.Address.Split('@');
                            return new SendSmtpEmailCc(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1]));
                        }).ToList(),
                Subject = mail.Subject
            };

            var emailHeaders = new Dictionary<string, string>();
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

            var attachments = new List<SendSmtpEmailAttachment>();
            // Adding the attachments
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

            //Adding Tags
            message.Tags = mail.Tags;

            message.Attachment = attachments.Count == 0 ? null : attachments;
            message.Headers = emailHeaders.Count == 0 ? null : emailHeaders;
            message.Bcc = message.Bcc.Count == 0 ? null : message.Bcc;
            message.Cc = message.Cc.Count == 0 ? null : message.Cc;

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
            var responses = new List<IMailResponse>();

            var completeEvent = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                _client.SendTransacEmail(mail);
                completeEvent.Set();
            });

            completeEvent.WaitOne();
            responses.Add(new SendInBlueMailResponse
            {
                Email = mailAttributes.From.Address,
                DeliveryStatus = DeliveryStatus.QUEUED
            });

            return responses;
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
            var responses = new List<IMailResponse>();

            await this._client.SendTransacEmailAsync(mail);

            responses.Add(new SendInBlueMailResponse
            {
                Email = mailAttributes.From.Address,
                DeliveryStatus = DeliveryStatus.QUEUED
            });

            return responses;
        }

        #endregion

        #region Private methods

        private void AsyncSendCompleted(MailAttributes mail)
        {
            this._interceptor.OnMailSent(mail);
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
