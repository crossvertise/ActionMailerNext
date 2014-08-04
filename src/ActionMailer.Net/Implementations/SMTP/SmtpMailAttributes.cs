using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ActionMailer.Net.Interfaces;
using ActionMailer.Net.Utils;
using AttachmentCollection = ActionMailer.Net.Utils.AttachmentCollection;

namespace ActionMailer.Net.Implementations.SMTP
{
    /// <summary>
    ///     Mailer for SMTP
    /// </summary>
    public class SmtpMailAttributes : IMailAttributes
    {

        /// <summary>
        /// 
        /// </summary>
        public SmtpMailAttributes()
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
        ///     Creates a MailMessage for the current SmtpMailAttribute instance.
        /// </summary>
        public MailMessage GenerateProspectiveMailMessage()
        {
            var mail = this;
            var message = new MailMessage();
            
            for (var i = 0; i < mail.To.Count(); i++)
                message.To.Add(mail.To[i]);

            for (var i = 0; i < mail.Cc.Count(); i++)
                message.CC.Add(mail.Cc[i]);

            for (var i = 0; i < mail.Bcc.Count(); i++)
                message.Bcc.Add(mail.Bcc[i]);

            for (var i = 0; i < mail.ReplyTo.Count(); i++)
                message.ReplyToList.Add(mail.ReplyTo[i]);

            // From is optional because it could be set in <mailSettings>
            if (!String.IsNullOrWhiteSpace(mail.From.Address))
                message.From = new MailAddress(mail.From.Address, mail.From.DisplayName);

            message.Subject = mail.Subject;

            foreach (var kvp in mail.Headers)
                message.Headers[kvp.Key] = kvp.Value;

            foreach (var kvp in mail.Attachments)
                message.Attachments.Add(kvp.Value);

            foreach (var kvp in mail.Attachments.Inline)
                message.Attachments.Add(kvp.Value);

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
        public IList<MailAddress> To { get; private set; }

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
        public IList<MailAddress> ReplyTo { get; private set; }

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
        ///     Any view you wish to add.  The key of this collection is what
        ///     the view should be named.
        /// </summary>
        public AlternativeViewCollection AlternateViews { get; private set; }
    }
}