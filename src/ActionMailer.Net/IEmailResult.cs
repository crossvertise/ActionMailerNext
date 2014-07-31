using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace ActionMailer.Net
{
    /// <summary>
    /// Interface for email results. Allows to send the message synchonously or asynchonously
    /// </summary>
    public interface IEmailResult
    {
        /// <summary>
        /// Sends your message.  This call will block while the message is being sent. (not recommended)
        /// </summary>
        void Deliver();

        /// <summary>
        /// Sends your message asynchronously.  This method does not block.  If you need to know
        /// when the message has been sent, then override the OnMailSent method in MailerBase which
        /// will not fire until the asyonchronous send operation is complete.
        /// </summary>
        void DeliverAsync();

        /// <summary>
        /// The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        MailMessage Mail { get; }

        /// <summary>
        /// The IMailSender instance that is used to deliver mail.
        /// </summary>
        IMailSender MailSender { get; }

        /// <summary>
        /// The default encoding used to send a message.
        /// </summary>
        Encoding MessageEncoding { get; }
    }
}
