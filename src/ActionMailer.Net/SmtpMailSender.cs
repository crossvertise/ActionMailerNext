using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ActionMailer.Net {
    /// <summary>
    /// Implements IMailSender by using System.Net.Mail.SmtpClient.
    /// </summary>
    public class SmtpMailSender : IMailSender {
        private readonly SmtpClient _client;

        /// <summary>
        /// Creates a new mail sender based on System.Net.Mail.SmtpClient
        /// </summary>
        public SmtpMailSender() : this(new SmtpClient()) { }

        /// <summary>
        /// Creates a new mail sender based on System.Net.Mail.SmtpClient
        /// </summary>
        /// <param name="client">The underlying SmtpClient instance to use.</param>
        public SmtpMailSender(SmtpClient client) {
            _client = client;
        }


        /// <summary>
        /// Sends mail synchronously.
        /// </summary>
        /// <param name="mail">The mail you wish to send.</param>
        public void Send(MailMessage mail) {
            _client.Send(mail);
        }

        /// <summary>
        /// Sends mail asynchronously using tasks.
        /// </summary>
        /// <param name="mail">The mail message you wish to send.</param>
        /// <param name="callbackTask">The callback task that will be fired when sending is complete.</param>
        public async void SendAsync(MailMessage mail,  Action<MailMessage> callbackTask)
        {

            var sendTask = _client.SendMailAsync(mail);
            await sendTask.ContinueWith(t => callbackTask(mail));
        }


        /// <summary>
        /// Destroys the underlying SmtpClient.
        /// </summary>
        public void Dispose() {
            _client.Dispose();
        }
    }
}
