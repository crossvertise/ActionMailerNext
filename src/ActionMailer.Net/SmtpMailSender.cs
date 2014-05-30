using System;
using System.Net.Mail;

namespace ActionMailer.Net {
    /// <summary>
    /// Implements IMailSender by using System.Net.Mail.SmtpClient.
    /// </summary>
    public class SmtpMailSender : IMailSender {
        private readonly SmtpClient _client;
        private Action<MailMessage> _callback;

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
        /// Sends mail asynchronously.
        /// </summary>
        /// <param name="mail">The mail you wish to send.</param>
        /// <param name="callback">The callback method to invoke when the send operation is complete.</param>
        public void SendAsync(MailMessage mail, Action<MailMessage> callback) {
            _callback = callback;
            _client.SendCompleted += new SendCompletedEventHandler(AsyncSendCompleted);
            _client.SendAsync(mail, mail);
        }

        private void AsyncSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            // unsubscribe from the event so _client can be GC'ed if necessary
            _client.SendCompleted -= AsyncSendCompleted;
            _callback(e.UserState as MailMessage);
        }

        /// <summary>
        /// Destroys the underlying SmtpClient.
        /// </summary>
        public void Dispose() {
            _client.Dispose();
        }
    }
}
