using System;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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
        /// Sends mail asynchronously using tasks.
        /// </summary>
        /// <param name="mail">The mail message you wish to send.</param>
        /// <param name="callbackTask">The callback task that will be fired when sending is complete.</param>
        void SendAsync(MailMessage mail, Action<MailMessage> callbackTask);
    }
}
