using System;
using System.Threading.Tasks;
using ActionMailer.Net.Interfaces;

namespace ActionMailer.Net
{
    /// <summary>
    ///     Some helpers used to dilver mail.  Reduces the need to repeat code.
    /// </summary>
    public class DeliveryHelper
    {
        private readonly IMailInterceptor _interceptor;
        private readonly IMailSender _sender;

        /// <summary>
        ///     Creates a new delivery helper to be used for sending messages.
        /// </summary>
        /// <param name="sender">The sender to use when delivering mail.</param>
        /// <param name="interceptor">The interceptor to report with while delivering mail.</param>
        public DeliveryHelper(IMailSender sender, IMailInterceptor interceptor)
        {
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            if (sender == null)
                throw new ArgumentNullException("sender");

            _sender = sender;
            _interceptor = interceptor;
        }

        /// <summary>
        ///     Sends the given email using the given
        /// </summary>
        /// <param name="async">Whether or not to use asynchronous delivery.</param>
        /// <param name="mail">The mail message to send.</param>
        public async Task<IMailAttributes> Deliver(bool async, IMailAttributes mail)
        {
            if (mail == null)
                throw new ArgumentNullException("mail");

            var mailContext = new MailSendingContext(mail);
            _interceptor.OnMailSending(mailContext);

            if (mailContext.Cancel)
                return null;

            if (async)
            {
                Task<IMailAttributes> sendtask = _sender.SendAsync(mail);
                await sendtask.ContinueWith(t => AsyncSendCompleted(t.Result));
                return mail;
            }

            _sender.Send(mail);
            return mail;
        }

        private void AsyncSendCompleted(IMailAttributes mail)
        {
            _interceptor.OnMailSent(mail);
        }
    }
}