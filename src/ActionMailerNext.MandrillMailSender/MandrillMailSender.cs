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
    public class MandrillMailSender : IMailSender
    {
        private MandrillApi _client;

        public MandrillMailSender() : this(ConfigurationManager.AppSettings["MandrillApiKey"]) { }

        public MandrillMailSender(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey",
                    "The AppSetting 'MandrillApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            _client = new MandrillApi(apiKey);
        }

        /// <summary>
        ///     Creates a MailMessage for the current MailAttribute instance.
        /// </summary>
        protected EmailMessage GenerateProspectiveMailMessage(MailAttributes mail)
        {
            //create base message
            var message = new EmailMessage
            {
                from_name = mail.From.DisplayName,
                from_email = mail.From.Address,
                to = mail.To.Union(mail.Cc).Select(t => new EmailAddress(t.Address, t.DisplayName)),
                bcc_address = mail.Bcc.Any() ? mail.Bcc.First().Address : null,
                subject = mail.Subject,
                important = mail.Priority == MailPriority.High ? true : false
            };

            // We need to set Reply-To as a custom header
            if (mail.ReplyTo.Any())
            {
                message.AddHeader("Reply-To", string.Join(" , ", mail.ReplyTo));
            }

            // Adding content to the message
            foreach (var view in mail.AlternateViews)
            {
                using (var reader = new StreamReader(view.ContentStream))
                {
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
                        name = mailAttachment.Name,
                        type = mailAttachment.ContentType.MediaType,
                    });
                }
            }

            message.attachments = attachments;

            return message;
        }

        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            var response = new List<IMailResponse>();

            var resp = _client.SendMessage(mail);
            response.AddRange(resp.Select(result => new MandrillMailResponse
            {
                Email = result.Email,
                Status = MandrillMailResponse.GetProspectiveStatus(result.Status.ToString()),
                RejectReason = result.RejectReason,
                Id = result.Id
            }));

            return response;
        }

        public virtual async Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            var response = new List<IMailResponse>();

            await _client.SendMessageAsync(mail).ContinueWith(x => response.AddRange(x.Result.Select(result => new MandrillMailResponse
            {
                Email = result.Email,
                Status = MandrillMailResponse.GetProspectiveStatus(result.Status.ToString()),
                RejectReason = result.RejectReason,
                Id = result.Id
            })));

            return response;
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}
