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
            if (!String.IsNullOrWhiteSpace(mail.FromAddress.Address))
                message.From = new MailAddress(mail.FromAddress.Address);

            message.Subject = mail.Subject;

            foreach (var kvp in mail.Headers)
                message.Headers[kvp.Key] = kvp.Value;

            foreach (var kvp in mail.Attachments)
                message.Attachments.Add(kvp.Value);

            foreach (var kvp in mail.Attachments.Inline)
                message.Attachments.Add(kvp.Value);

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