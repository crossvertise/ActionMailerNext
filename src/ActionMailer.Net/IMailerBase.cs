using System.Collections.Generic;
using System.Text;

namespace ActionMailer.Net {
    /// <summary>
    /// All mailers should implement this interface.
    /// </summary>
    public interface IMailerBase : IMailInterceptor {
        /// <summary>
        /// A string representation of who this mail should be from.  Could be
        /// your name and email address or just an email address by itself.
        /// </summary>
        string From { get; set; }

        /// <summary>
        /// The subject line of the email.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// A collection of addresses this email should be sent to.
        /// </summary>
        IList<string> To { get; }

        /// <summary>
        /// A collection of addresses that should be CC'ed.
        /// </summary>
        IList<string> CC { get; }
        
        /// <summary>
        /// A collection of addresses that should be BCC'ed.
        /// </summary>
        IList<string> BCC { get; }

        /// <summary>
        /// A collection of addresses that should be listed in Reply-To header.
        /// </summary>
        IList<string> ReplyTo { get; }

        /// <summary>
        /// Any custom headers (name and value) that should be placed on the message.
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets or sets the default message encoding when delivering mail.
        /// </summary>
        Encoding MessageEncoding { get; set; }

        /// <summary>
        /// Any attachments you wish to add.  The key of this collection is what
        /// the file should be named.  The value is should represent the binary bytes
        /// of the file.
        /// </summary>
        /// <example>
        /// Attachments["picture.jpg"] = File.ReadAllBytes(@"C:\picture.jpg");
        /// </example>
        AttachmentCollection Attachments { get; }

        /// <summary>
        /// The underlying IMailSender to use for outgoing messages.
        /// </summary>
        IMailSender MailSender { get; set; }
    }
}
