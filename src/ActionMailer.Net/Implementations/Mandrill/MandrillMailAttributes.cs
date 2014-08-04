using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using ActionMailer.Net.Interfaces;
using ActionMailer.Net.Utils;
using Mandrill;
using AttachmentCollection = ActionMailer.Net.Utils.AttachmentCollection;

namespace ActionMailer.Net.Implementations.Mandrill
{
    /// <summary>
    /// </summary>
    public class MandrillMailAttributes : IMailAttributes
    {

        /// <summary>
        /// 
        /// </summary>
        public MandrillMailAttributes()
        {
            To = new List<MailAddress>();
            Cc = new List<MailAddress>();
            Bcc = new List<MailAddress>();
            ReplyTo = new List<MailAddress>();
            Attachments = new AttachmentCollection();
            AlternateViews = new AlternativeViewCollection();
            Headers = new Dictionary<string, string>();
        }

      

        /// <summary>
        ///     Creates a EmailMessage for the given IMailAttributes instance.
        /// </summary>
        public EmailMessage GenerateProspectiveMailMessage()
        {
            var mail = this;

            if (mail.Cc.Any())
                throw new NotSupportedException("The CC field is not supported with the MandrillMailSender");

            if (mail.ReplyTo.Any())
                throw new NotSupportedException("The ReplyTo field is not supported with the MandrillMailSender");

            //create base message
            var message = new EmailMessage
            {
                from_name = mail.FromName,
                from_email = mail.FromAddress.Address,
                to = mail.To.Select(t => new EmailAddress(t.Address, t.DisplayName)),
                bcc_address = mail.Bcc.Any() ? mail.Bcc.First().Address : null,
                subject = mail.Subject,
            };

            //add headers
            foreach (var kvp in mail.Headers)
                message.headers[kvp.Key] = kvp.Value;

            //add content
            foreach (var view in mail.AlternateViews)
            {
                using (var reader = new StreamReader(view.Value.ContentStream))
                {
                    var body = reader.ReadToEnd();

                    if (view.Value.ContentType.MediaType == MediaTypeNames.Text.Plain)
                        message.text = body;

                    if (view.Value.ContentType.MediaType == MediaTypeNames.Text.Html)
                        message.html = body;
                }
            }

            //add attachments
            var atts = new List<attachment>();
            foreach (var attachment in mail.Attachments)
            {
                using (var stream = new MemoryStream())
                {
                    attachment.Value.ContentStream.CopyTo(stream);
                    var base64Data = Convert.ToBase64String(stream.ToArray());

                    atts.Add(new attachment
                    {
                        content = base64Data,
                        name = attachment.Value.ContentId,
                        type = attachment.Value.ContentType.MediaType,
                    });
                }
            }
            message.attachments = atts;

            return message;
        }

        public string FromName { get; set; }
        public MailAddress FromAddress { get; set; }
        public string Subject { get; set; }
        public MailPriority Priority { get; set; }
        public IList<MailAddress> To { get; private set; }
        public IList<MailAddress> Cc { get; private set; }
        public IList<MailAddress> Bcc { get; private set; }
        public IList<MailAddress> ReplyTo { get; private set; }
        public IDictionary<string, string> Headers { get; private set; }
        public Encoding MessageEncoding { get; set; }
        public AttachmentCollection Attachments { get; private set; }
        public AlternativeViewCollection AlternateViews { get; private set; }
    }
}