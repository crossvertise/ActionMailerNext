using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ActionMailerNext.Interfaces;
using AttachmentCollection = ActionMailerNext.Utils.AttachmentCollection;

namespace ActionMailerNext.Implementations.SMTP
{
    /// <summary>
    ///     Mailer for SMTP
    /// </summary>
    public class SmtpMailAttributes : IMailAttributes
    {
        /// <summary>
        /// </summary>
        public SmtpMailAttributes()
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
        ///     Creates a MailMessage for the current SmtpMailAttribute instance.
        /// </summary>
        public MailMessage GenerateProspectiveMailMessage()
        {
            var mail = this;
            var message = new MailMessage();

            for (int i = 0; i < mail.To.Count(); i++)
                message.To.Add(mail.To[i]);

            for (int i = 0; i < mail.Cc.Count(); i++)
                message.CC.Add(mail.Cc[i]);

            for (int i = 0; i < mail.Bcc.Count(); i++)
                message.Bcc.Add(mail.Bcc[i]);

            for (int i = 0; i < mail.ReplyTo.Count(); i++)
                message.ReplyToList.Add(mail.ReplyTo[i]);

            // From is optional because it could be set in <mailSettings>
            if (!String.IsNullOrWhiteSpace(mail.From.Address))
                message.From = new MailAddress(mail.From.Address, mail.From.DisplayName);

            message.Subject = mail.Subject;
            message.BodyEncoding = mail.MessageEncoding;
            message.Priority = mail.Priority;

            foreach (var kvp in mail.Headers)
                message.Headers[kvp.Key] = kvp.Value;

            foreach (var kvp in mail.Attachments)
                message.Attachments.Add(AttachmentCollection.ModifyAttachmentProperties(kvp.Key, kvp.Value, false));

            foreach (var kvp in mail.Attachments.Inline)
                message.Attachments.Add(AttachmentCollection.ModifyAttachmentProperties(kvp.Key, kvp.Value, true));

            foreach (AlternateView view in AlternateViews)
                message.AlternateViews.Add(view);

            return message;
        }
    }
}