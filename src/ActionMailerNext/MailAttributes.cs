using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ActionMailerNext.Interfaces;
using CsQuery.ExtensionMethods.Internal;
using AttachmentCollection = ActionMailerNext.Utils.AttachmentCollection;

namespace ActionMailerNext
{
    /// <summary>
    ///     All mailers should implement this interface.
    /// </summary>
    public class MailAttributes
    {

        public MailAttributes()
        {
            To = new List<MailAddress>();
            Cc = new List<MailAddress>();
            Bcc = new List<MailAddress>();
            ReplyTo = new List<MailAddress>();
            Headers = new Dictionary<string, string>();
            Attachments = new AttachmentCollection();
            AlternateViews = new List<AlternateView>();
            ExtraProperties = new Dictionary<string, string>();
            PostProcessors = new List<IPostProcessor>();
        }

        public MailAttributes(MailAttributes mailAttributes, 
            bool copyTo = true, bool copyCc = true,
            bool copyBcc = true, bool copyReplyTo = true, bool referenceAttachments = true,
            bool copyHeaders = true, bool copyExtraProperties = true, 
            bool copyPostProcessors = true)
        {

            From = mailAttributes.From;
            Subject = mailAttributes.Subject;
            Priority = mailAttributes.Priority;
            
            IsCcToSupported = mailAttributes.IsCcToSupported;
            IsBccSupported = mailAttributes.IsBccSupported;
            IsReplyToSupported = mailAttributes.IsReplyToSupported;
            
            MessageEncoding = mailAttributes.MessageEncoding;
            Body = mailAttributes.Body;
            TextBody = mailAttributes.TextBody;
            HtmlBody = mailAttributes.HtmlBody;

            To = copyTo ? mailAttributes.To.Select(mailAddress => new MailAddress(mailAddress.Address, mailAddress.DisplayName))
                .ToList() : new List<MailAddress>();
            Cc = copyCc ? mailAttributes.Cc.Select(mailAddress => new MailAddress(mailAddress.Address, mailAddress.DisplayName))
                    .ToList() : new List<MailAddress>();
            Bcc = copyBcc ? mailAttributes.Bcc.Select(mailAddress => new MailAddress(mailAddress.Address, mailAddress.DisplayName))
                    .ToList() : new List<MailAddress>();
            ReplyTo = copyReplyTo ? mailAttributes.ReplyTo.Select(mailAddress => new MailAddress(mailAddress.Address, mailAddress.DisplayName))
                    .ToList() : new List<MailAddress>();
            Attachments = referenceAttachments ? mailAttributes.Attachments : new AttachmentCollection();
            Headers = copyHeaders ? mailAttributes.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<string, string>();
            ExtraProperties = copyExtraProperties ? mailAttributes.ExtraProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : new Dictionary<string, string>();
            PostProcessors = copyPostProcessors ? mailAttributes.PostProcessors.Select(pp => pp).ToList() : new List<IPostProcessor>();

            AlternateViews = new List<AlternateView>();

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
        public List<MailAddress> To { get; set; }

        /// <summary>
        ///  Check if the current Sending method supports CC
        /// </summary>
        public bool IsCcToSupported { get; set; }

        /// <summary>
        ///     A collection of addresses that should be CC'ed.
        /// </summary>
        public IList<MailAddress> Cc { get; set;}

        /// <summary>
        ///  Check if the current Sending method supports BCC
        /// </summary>
        public bool IsBccSupported { get; set; }

        /// <summary>
        ///     A collection of addresses that should be BCC'ed.
        /// </summary>
        public IList<MailAddress> Bcc { get; set; }

        /// <summary>
        ///  Check if the current Sending method supports ReplyTo
        /// </summary>
        public bool IsReplyToSupported { get; set; }

        /// <summary>
        ///     A collection of addresses that should be listed in Reply-To header.
        /// </summary>
        public List<MailAddress> ReplyTo { get; set; }

        /// <summary>
        ///     Any custom headers (name and value) that should be placed on the message.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }
        
        /// <summary>
        ///     The generated text body of the message
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        ///     The generated html body of the message
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        ///     The generated body of the message
        /// </summary>
        public string Body { get; set; }

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
        public AttachmentCollection Attachments { get; set; }

        /// <summary>
        ///     Any view you wish to add.  The key of this collection is what
        ///     the view should be named.
        /// </summary>
        public IList<AlternateView> AlternateViews { get; set; }

        /// <summary>
        ///     Apply PreMailer.Net to convert all styles to inline styles to 
        ///     avoid problems with different Email Clients
        /// </summary>
        public IList<IPostProcessor> PostProcessors { get; set; }

        /// <summary>
        ///     Any extra properties that needs to be added in case of custom mail sender
        /// </summary>
        public IDictionary<String,String> ExtraProperties { get; set; }
    }
}