using System;
using System.Net.Mail;

namespace ActionMailer.Net {
    /// <summary>
    /// Some helpers used to dilver mail.  Reduces the need to repeat code.
    /// </summary>
    public class DeliveryHelper {
        private IMailSender _sender;
        private IMailInterceptor _interceptor;

        /// <summary>
        /// Creates a new dilvery helper to be used for sending messages.
        /// </summary>
        /// <param name="sender">The sender to use when delivering mail.</param>
        /// <param name="interceptor">The interceptor to report with while delivering mail.</param>
        public DeliveryHelper(IMailSender sender, IMailInterceptor interceptor) {
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            if (sender == null)
                throw new ArgumentNullException("sender");

            _sender = sender;
            _interceptor = interceptor;
        }

        /// <summary>
        /// Sends the given email using the given
        /// </summary>
        /// <param name="async">Whether or not to use asynchronous delivery.</param>
        /// <param name="mail">The mail message to send.</param>
        public void Deliver(bool async, MailMessage mail) {
            if (mail == null)
                throw new ArgumentNullException("mail");

            var mailContext = new MailSendingContext(mail);
            _interceptor.OnMailSending(mailContext);

            if (mailContext.Cancel)
                return;

            if (async) {
                _sender.SendAsync(mail, AsyncSendCompleted);
                return;
            }

            _sender.Send(mail);
            _interceptor.OnMailSent(mail);
        }

        private void AsyncSendCompleted(MailMessage mail) {
            _interceptor.OnMailSent(mail);
        }
    }
}
