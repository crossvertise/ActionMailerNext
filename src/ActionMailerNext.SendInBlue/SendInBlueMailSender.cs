namespace ActionMailerNext.SendInBlue
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading.Tasks;

    using sib_api_v3_sdk.Api;
    using sib_api_v3_sdk.Model;
    using SibClient = sib_api_v3_sdk.Client;
    using ActionMailerNext.Interfaces;

    public class SendInBlueMailSender : IMailSender
    {
        private readonly IMailInterceptor _interceptor;
        private readonly TransactionalEmailsApi _client;

        public SendInBlueMailSender() : this(ConfigurationManager.AppSettings["SendInBlueApiKey"], null) { }

        public SendInBlueMailSender(string apiKey, IMailInterceptor interceptor)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey),
                    "The AppSetting 'SendInBlueApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");
            }

            _interceptor = interceptor;
            if (!SibClient.Configuration.Default.ApiKey.ContainsKey("api-key"))
            {
                SibClient.Configuration.Default.ApiKey.Add("api-key", apiKey);
            }

            _client = new TransactionalEmailsApi();
        }

        public SendSmtpEmail GenerateProspectiveMailMessage(MailAttributes mail)
        {
            var idnmapping = new IdnMapping();

            string parseEmail(string address)
            {
                var parts = address.Split('@');
                return $"{parts[0]}@{idnmapping.GetAscii(parts[1])}";
            }

            var message = new SendSmtpEmail
            {
                Sender = new SendSmtpEmailSender(mail.From.DisplayName, mail.From.Address),
                To = mail.To.Select(t => new SendSmtpEmailTo(parseEmail(t.Address))).ToList(),
                Bcc = mail.Bcc.Select(t => new SendSmtpEmailBcc(parseEmail(t.Address))).ToList(),
                Cc = mail.Cc.Select(t => new SendSmtpEmailCc(parseEmail(t.Address))).ToList(),
                Subject = mail.Subject
            };

            var emailHeaders = new Dictionary<string, string>();
            if (mail.ReplyTo.Any())
            {
                emailHeaders.Add("Reply-To", string.Join(" , ", mail.ReplyTo));
            }

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

            mail.Headers.ToList().ForEach(h => emailHeaders.Add(h.Key, h.Value));

            var attachments = new List<SendSmtpEmailAttachment>(mail.Attachments.Count);

            foreach (var mailAttachment in mail.Attachments.Select(attachment =>
                Utils.AttachmentCollection.ModifyAttachmentProperties(attachment.Key, attachment.Value, false)))
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

            message.Tags = mail.Tags;
            message.Attachment = attachments.Count == 0 ? null : attachments;
            message.Headers = emailHeaders.Count == 0 ? null : emailHeaders;
            message.Bcc = message.Bcc.Count == 0 ? null : message.Bcc;
            message.Cc = message.Cc.Count == 0 ? null : message.Cc;
            message.Tags = message.Tags.Count == 0 ? null : message.Tags;

            return message;
        }

        public virtual List<IMailResponse> Deliver(IEmailResult emailResult)
        {
            try
            {
                return Send(emailResult.MailAttributes);
            }
            catch (Exception ex)
            {
                throw new SendInBlueException(ex.Message, ex);
            }
        }

        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            var responses = new List<IMailResponse>();

            _client.SendTransacEmail(mail);

            responses.Add(new SendInBlueMailResponse
            {
                Email = mailAttributes.From.Address,
                DeliveryStatus = DeliveryStatus.QUEUED
            });

            return responses;
        }

        /// <summary>
        /// Sends your message asynchronously.  This method does not block.  If you need to know
        /// when the message has been sent, then override the OnMailSent method in MailerBase which
        /// will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public async Task<MailAttributes> DeliverAsync(IEmailResult emailResult)
        {
            try
            {
                await SendAsync(emailResult.MailAttributes);
                AsyncSendCompleted(emailResult.MailAttributes);

                return emailResult.MailAttributes;
            }
            catch (Exception ex)
            {
                throw new SendInBlueException(ex.Message, ex);
            }
        }

        public virtual async Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            var responses = new List<IMailResponse>();

            await _client.SendTransacEmailAsync(mail);

            responses.Add(new SendInBlueMailResponse
            {
                Email = mailAttributes.From.Address,
                DeliveryStatus = DeliveryStatus.QUEUED
            });

            return responses;
        }

        private void AsyncSendCompleted(MailAttributes mail)
        {
            _interceptor.OnMailSent(mail);
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

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
