using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Net.Mime;
using Mandrill;
using ActionMailerNext.Interfaces;

namespace ActionMailerNext.MandrillMailSender
{
    using System.Globalization;
    using System.Text;

    public class MandrillMailSender : IMailSender
    {
        private readonly IMailInterceptor interceptor;
        private readonly MandrillApi client;

        public MandrillMailSender() : this(ConfigurationManager.AppSettings["MandrillApiKey"], null) { }

        public MandrillMailSender(string apiKey, IMailInterceptor interceptor)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey",
                    "The AppSetting 'MandrillApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            this.interceptor = interceptor;
            this.client = new MandrillApi(apiKey);
        }

        /// <summary>
        ///     Creates a MailMessage for the current MailAttribute instance.
        /// </summary>
        protected EmailMessage GenerateProspectiveMailMessage(MailAttributes mail)
        {
            var idnmapping = new IdnMapping();

            var emailAddresses = mail.To
                .Select(
                    t =>
                    {
                        var domainSplit = t.Address.Split('@');
                        return new EmailAddress(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1])) { type = "to" };
                    })
                .Union(
                    mail.Cc.Select(
                        t =>
                        {
                            var domainSplit = t.Address.Split('@');
                            return new EmailAddress(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1])) { type = "cc" };
                        }))
                .Union(
                    mail.Bcc.Select(
                        t =>
                        {
                            var domainSplit = t.Address.Split('@');
                            return new EmailAddress(domainSplit[0] + "@" + idnmapping.GetAscii(domainSplit[1])) { type = "bcc" };
                        }));       

            //create base message
            var message = new EmailMessage
            {
                from_name = mail.From.DisplayName,
                from_email = mail.From.Address,
                to = emailAddresses,
                subject = mail.Subject,
                important = mail.Priority == MailPriority.High,
                preserve_recipients = true
            };

            // We need to set Reply-To as a custom header
            if (mail.ReplyTo.Any())
            {
                message.AddHeader("Reply-To", string.Join(" , ", mail.ReplyTo));
            }

            // Adding content to the message
            foreach (var view in mail.AlternateViews)
            {
                var reader = new StreamReader(view.ContentStream, Encoding.UTF8, true, 1024, true);
                
                    var body = reader.ReadToEnd();

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Plain)
                    {
                        message.text = body;
                    }
                    if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                    {
                        message.html = body;
                    }      
            }

            // Going through headers and adding them to the message
            mail.Headers.ToList().ForEach(h => message.AddHeader(h.Key, h.Value));

            // Adding the attachments
            var attachments = new List<email_attachment>();
            foreach (var mailAttachment in mail.Attachments.Select(attachment => Utils.AttachmentCollection.ModifyAttachmentProperties(attachment.Key, attachment.Value, false)))
            {
                using (var stream = new MemoryStream())
                {
                    mailAttachment.ContentStream.CopyTo(stream);
                    var base64Data = Convert.ToBase64String(stream.ToArray());
                    attachments.Add(new email_attachment
                    {
                        content = base64Data,
                        name = ReplaceGermanCharacters(mailAttachment.Name),
                        type = mailAttachment.ContentType.MediaType,
                    });
                }
            }

            message.attachments = attachments;

            return message;
        }

        #region Send methods

        public virtual List<IMailResponse> Deliver(IEmailResult emailResult) {
            return this.Send(emailResult.MailAttributes);
        }

        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            var response = new List<IMailResponse>();

            var resp = this.client.SendMessage(mail);
            response.AddRange(resp.Select(result => new MandrillMailResponse
            {
                Email = result.Email,
                Status = MandrillMailResponse.GetProspectiveStatus(result.Status.ToString()),
                RejectReason = result.RejectReason,
                Id = result.Id
            }));

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

            await this.client.SendMessageAsync(mail).ContinueWith(x => response.AddRange(x.Result.Select(result => new MandrillMailResponse
            {
                Email = result.Email,
                Status = MandrillMailResponse.GetProspectiveStatus(result.Status.ToString()),
                RejectReason = result.RejectReason,
                Id = result.Id
            })));

            return response;
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
