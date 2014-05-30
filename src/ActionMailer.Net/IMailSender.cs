using System;
using System.Net.Mail;

namespace ActionMailer.Net {
    /// <summary>
    /// An object used to deliver mail.
    /// </summary>
    public interface IMailSender : IDisposable {
        /// <summary>
        /// Sends mail synchronously.
        /// </summary>
        /// <param name="mail">The mail message you wish to send.</param>
        void Send(MailMessage mail);

        /// <summary>
        /// Sends mail asynchronously.
        /// </summary>
        /// <param name="mail">The mail message you wish to send.</param>
        /// <param name="callback">The callback method that will be fired when sending is complete.</param>
        void SendAsync(MailMessage mail, Action<MailMessage> callback);
    }
}
