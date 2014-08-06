using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using ActionMailer.Net.Interfaces;
using ActionMailer.Net.Utils;
using Mandrill;
using RestSharp;
using AttachmentCollection = ActionMailer.Net.Utils.AttachmentCollection;

namespace ActionMailer.Net.Implementations.Mandrill
{
    /// <summary>
    ///     Mailer for Mandrill.
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
            AlternateViews = new List<AlternateView>();
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
                from_name = mail.From.DisplayName,
                from_email = mail.From.Address,
                to = mail.To.Select(t => new EmailAddress(t.Address, t.DisplayName)),
                bcc_address = mail.Bcc.Any() ? mail.Bcc.First().Address : null,
                subject = mail.Subject
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
                        message.text = body;

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                        message.html = body;
                }
            }

            //add attachments
            var atts = new List<attachment>();
            foreach (var attachment in mail.Attachments)
            {
                var mailAttachment = AttachmentCollection.ModifyAttachmentProperties(attachment.Key, attachment.Value,
                    false);
                using (var stream = new MemoryStream())
                {
                    mailAttachment.ContentStream.CopyTo(stream);
                    var base64Data = Convert.ToBase64String(stream.ToArray());
                    atts.Add(new attachment
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
        ///     A string representation of who this mail should be from.  Could be
        ///     your name and email address or just an email address by itself.
        /// </summary>
        public MailAddress From { get; set; }

        /// <summary>
        ///     The subject line of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///     The Priority of the email.
        /// </summary>
        public MailPriority Priority { get; set; }

        /// <summary>
        ///     A collection of addresses this email should be sent to.
        /// </summary>
        public List<MailAddress> To { get; private set; }

        /// <summary>
        ///     A collection of addresses that should be CC'ed.
        /// </summary>
        public IList<MailAddress> Cc { get; private set; }

        /// <summary>
        ///     A collection of addresses that should be BCC'ed.
        /// </summary>
        public IList<MailAddress> Bcc { get; private set; }

        /// <summary>
        ///     A collection of addresses that should be listed in Reply-To header.
        /// </summary>
        public List<MailAddress> ReplyTo { get; private set; }

        /// <summary>
        ///     Any custom headers (name and value) that should be placed on the message.
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        ///     Gets or sets the default message encoding when delivering mail.
        /// </summary>
        public Encoding MessageEncoding { get; set; }

        /// <summary>
        ///     Any attachments you wish to add.  The key of this collection is what
        ///     the file should be named.  The value is should represent the actual content
        ///     of the file.
        /// </summary>
        /// <example>
        ///     Attachments["picture.jpg"] = File.ReadAllBytes(@"C:\picture.jpg");
        /// </example>
        public AttachmentCollection Attachments { get; private set; }

        /// <summary>
        ///     Any view you wish to add.  The key of this collection is what
        ///     the view should be named.
        /// </summary>
        public IList<AlternateView> AlternateViews { get; private set; }
    }
}