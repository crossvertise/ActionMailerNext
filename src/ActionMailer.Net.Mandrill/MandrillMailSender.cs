using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Threading.Tasks;
using Mandrill;

namespace ActionMailer.Net.Mandrill
{
    /// <summary>
    /// Implementation of IMailSender that allows sending to Mandrill via HTTP
    /// </summary>
    public class MandrillMailSender : IMailSender
    {
        private MandrillApi _mandrillClient;

        /// <summary>
        /// Create a new instance of the MandrillMailSender using an API key provided in AppSettings as key MandrillApiKey.
        /// </summary>
        public MandrillMailSender()
        {
            var apiKey = ConfigurationManager.AppSettings["MandrillApiKey"];
            if(string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("The AppSetting 'MandrillApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            _mandrillClient = new MandrillApi(apiKey);
        }

        /// <summary>
        /// Create a new instance with the given API key
        /// </summary>
        /// <param name="apiKey">The Mandrill API key, e.g. "PmmzuovUZMPJsa73o3jjCw"</param>
        public MandrillMailSender(string apiKey)
        {
            _mandrillClient = new MandrillApi(apiKey);
        }
        public void Dispose()
        {
            _mandrillClient = null;
        }

        /// <summary>
        /// Sends the message synchonously through the API
        /// </summary>
        /// <param name="mail">The mail to be sent</param>
        public void Send(MailMessage mail)
        {
            var message = ToMandrillMessage(mail);
            _mandrillClient.SendMessage(message);
        }

        /// <summary>
        /// Sends the message asyncronously through the API and returns before the mail is delivered
        /// </summary>
        /// <param name="mail">The mail to be sent</param>
        /// <param name="callback">a callback executed when the sending finished</param>
        public void SendAsync(MailMessage mail, Action<MailMessage> callback)
        {
            var message = ToMandrillMessage(mail);
            _mandrillClient.SendMessageAsync(message).ContinueWith( a => callback(mail) );
        }

        //public async Task<List<EmailResult>> SendAsncTask(MailMessage mail)
        //{
        //      var message = ToMandrillMessage(mail);
        //    return await _mandrillClient.SendMessageAsync(message);
        //}


        /// <summary>
        /// Converts a .NET MailMessage to the required API proxy class 
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
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
