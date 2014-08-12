using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using ActionMailerNext.Interfaces;
using SendGrid;
using AttachmentCollection = ActionMailerNext.Utils.AttachmentCollection;

namespace ActionMailerNext.Implementations.SendGrid
{
    /// <summary>
    ///     Mailer for SMTP
    /// </summary>
    public class SendGridMailAttributes : IMailAttributes
    {
        /// <summary>
        /// </summary>
        public SendGridMailAttributes()
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
        ///     The generated text body of the message
        /// </summary>
        public string TextBody
        {
            get
            {
                foreach (var view in AlternateViews)
                {
                    using (var reader = new StreamReader(view.ContentStream))
                    {
                        var body = reader.ReadToEnd();
                        if (view.ContentType.MediaType == MediaTypeNames.Text.Plain)
                        {
                            return body;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///     The generated html body of the message
        /// </summary>
        public string HTMLBody
        {
            get
            {
                foreach (var view in AlternateViews)
                {
                    using (var reader = new StreamReader(view.ContentStream))
                    {
                        var body = reader.ReadToEnd();
                        if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                        {
                            return body;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///     The generated body of the message
        /// </summary>
        public string Body
        {
            get { return TextBody ?? HTMLBody; }
        }

        /// <summary>
        ///     Gets or sets the default message encoding when delivering mail.
        /// </summary>
        public Encoding MessageEncoding { get; set; }

        /// <summary>
        ///     Any attachments you wish to add.  The key of this collection is what
        ///     the file should be named.  The value is should represent the binary bytes
        ///     of the file.
        /// </summary>
        /// <example>
        ///     Attachments["picture.jpg"] = File.ReadAllBytes(@"C:\picture.jpg");
        /// </example>
        public AttachmentCollection Attachments { get; private set; }

        /// <summary>
        ///     Any view you wish to add.
        /// </summary>
        public IList<AlternateView> AlternateViews { get; private set; }

        /// <summary>
        ///     Creates a SendGridMessage for the current SendGridMailAttributes instance.
        /// </summary>
        public SendGridMessage GenerateProspectiveMailMessage()
        {
            var mail = this;

            if (mail.Cc.Any())
                throw new NotSupportedException("The CC field is not supported with the SendGridMailSender");

            if (mail.Bcc.Any())
                throw new NotSupportedException("The ReplyTo field is not supported with the SendGridMailSender");

            //create base message
            var message = new SendGridMessage
            {
                From = mail.From,
                To = mail.To.ToArray(),
                ReplyTo = mail.ReplyTo.ToArray(),
                Subject = mail.Subject,
                Headers = (Dictionary<string,string>)mail.Headers
            };

            //add content
            foreach (var view in mail.AlternateViews)
            {
                using (var reader = new StreamReader(view.ContentStream))
                {
                    var body = reader.ReadToEnd();

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Plain)
                    {
                        message.Text = body;
                    }
                    if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                    {
                        message.Html = body;
                    }
                }
            }

            //add attachments
            foreach (var mailAttachment in mail.Attachments.Select(attachment => AttachmentCollection.ModifyAttachmentProperties(attachment.Key,
                attachment.Value,
                false)))
            {
                using (var stream = new MemoryStream())
                {
                    mailAttachment.ContentStream.CopyTo(stream);
                    message.AddAttachment(stream, mailAttachment.Name);
                }
            }

            return message;
        }
    }
}