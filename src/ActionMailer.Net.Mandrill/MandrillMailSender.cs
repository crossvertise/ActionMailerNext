using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Mandrill;

namespace ActionMailer.Net.Mandrill
{
    public class MandrillMailSender : IMailSender
    {
        private MandrillApi _mandrillClient;

        public MandrillMailSender()
        {
            var apiKey = ConfigurationManager.AppSettings["MandrillApiKey"];
            if(string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("The AppSetting 'MandrillApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            _mandrillClient = new MandrillApi(apiKey);
        }
        public MandrillMailSender(string apiKey)
        {
            _mandrillClient = new MandrillApi(apiKey);
        }
        public void Dispose()
        {
            _mandrillClient = null;
        }

        public void Send(MailMessage mail)
        {
            var message = ToMandrillMessage(mail);
            _mandrillClient.SendMessage(message);
        }

        public void SendAsync(MailMessage mail, Action<MailMessage> callback)
        {
            var message = ToMandrillMessage(mail);
            _mandrillClient.SendMessageAsync(message).ContinueWith( a => callback(mail) );
        }

        private static EmailMessage ToMandrillMessage(MailMessage mail)
        {
            if (mail.CC.Any())
                throw new NotSupportedException("The CC field is not supported with the MandrillMailSender");

            if (mail.ReplyToList.Any())
                throw new NotSupportedException("The ReplyTo field is not supported with the MandrillMailSender");

            //create base message
            var message = new EmailMessage
            {
                from_name = mail.From.Address,
                from_email = mail.From.DisplayName,
                to = mail.To.Select(t => new EmailAddress(t.Address, t.DisplayName)),
                bcc_address = mail.Bcc.Any() ? mail.Bcc.First().Address : null,
                subject = mail.Subject,
            };

            //add headers
            for (int i = 0; i < mail.Headers.Count; i++)
            {
                message.AddHeader(mail.Headers.Keys[i], mail.Headers[i]);
            }

            //add content
            foreach (var view in mail.AlternateViews)
            {
                using (var reader = new StreamReader(view.ContentStream))
                {
                    var body = reader.ReadToEnd();

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Plain)
                        message.text = body;

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                        message.html = body;
                }
            }

            //add attachments
            var atts = new List<attachment>();
            foreach (var attachment in mail.Attachments)
            {
                using (var stream = new MemoryStream())
                {
                    attachment.ContentStream.CopyTo(stream);
                    var base64Data = Convert.ToBase64String(stream.ToArray());

                    atts.Add(new attachment
                    {
                        content = base64Data,
                        name = attachment.ContentId,
                        type = attachment.ContentType.MediaType,
                    });
                }
            }
            message.attachments = atts;

            return message;
        }
    }
}
