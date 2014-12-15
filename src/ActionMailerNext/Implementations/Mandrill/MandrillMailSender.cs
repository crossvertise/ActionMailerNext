using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;
using Mandrill;

namespace ActionMailerNext.Implementations.Mandrill
{
    /// <summary>
    ///     Implements IMailSender by using Mandrill.MandrillApi
    /// </summary>
    public class MandrillMailSender : IMailSender
    {
        private MandrillApi _client;

        /// <summary>
        ///     Creates a new MandrillMailSender based on Mandrill.MandrillApi
        /// </summary>
        public MandrillMailSender() : this(ConfigurationManager.AppSettings["MandrillApiKey"]){}

        /// <summary>
        ///     Creates a new MandrillApi based onMandrill.MandrillApi
        /// </summary>
        /// <param name="apiKey"></param>
        public MandrillMailSender(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey",
                    "The AppSetting 'MandrillApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            _client = new MandrillApi(apiKey);
        }

        /// <summary>
        ///     Creates a EmailMessage for the given MandrillMailAttributes instance.
        /// </summary>
        protected EmailMessage GenerateProspectiveMailMessage(MailAttributes mail)
        {

            if (mail.Cc.Any())
                throw new NotSupportedException("The CC field is not supported with the MandrillMailSender");

            if (mail.ReplyTo.Any())
                throw new NotSupportedException("The ReplyTo field is not supported with the MandrillMailSender");

            //create base message
            var message = new EmailMessage
            {
                from_name = mail.From.DisplayName,
                from_email = mail.From.Address,
                to = mail.To.Select(t => new EmailAddress(t.Address, t.DisplayName)),
                bcc_address = mail.Bcc.Any() ? mail.Bcc.First().Address : null,
                subject = mail.Subject,
                important = mail.Priority == MailPriority.High
            };


            //add headers
            foreach (var kvp in mail.Headers)
                message.AddHeader(kvp.Key, kvp.Value);

            //add content
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

            //add attachments
            var atts = new List<email_attachment>();
            foreach (var mailAttachment in mail.Attachments.Select(attachment => Utils.AttachmentCollection.ModifyAttachmentProperties(attachment.Key,
                attachment.Value,
                false)))
            {
                using (var stream = new MemoryStream())
                {

                    mailAttachment.ContentStream.CopyTo(stream);
                    var base64Data = Convert.ToBase64String(stream.ToArray());
                    atts.Add(new email_attachment
                    {
                        content = base64Data,
                        name = mailAttachment.Name,
                        type = mailAttachment.ContentType.MediaType,
                    });
                }
            }
            message.attachments = atts;

            return message;
        }

        /// <summary>
        ///     Sends MandrillMessage synchronously.
        /// </summary>
        /// <param name="mailAttributes">The MailAttributes you wish to send.</param>
        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {

            var mail = GenerateProspectiveMailMessage(mailAttributes);
            var response = new List<IMailResponse>();
            
            var re = _client.SendMessage(mail);
            response.AddRange(re.Select(result => new MandrillMailResponse
            {
                Email = result.Email,
                Status = MandrillMailResponse.GetProspectiveStatus(result.Status.ToString()),
                RejectReason = result.RejectReason,
                Id = result.Id
            }));

            return response;
        }

        /// <summary>
        ///     Sends MandrillMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The MailAttributes message you wish to send.</param>
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
        
        /// <summary>
        ///     Destroys the underlying MandrillApi.
        /// </summary>
        public void Dispose()
        {
            _client = null;
        }
    }
}