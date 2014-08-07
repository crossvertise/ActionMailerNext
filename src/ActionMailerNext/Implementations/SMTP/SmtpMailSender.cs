using System.Net.Mail;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;

namespace ActionMailerNext.Implementations.SMTP
{
    /// <summary>
    ///     Implements IMailSender by using System.Net.Mail.SmtpClient.
    /// </summary>
    public class SmtpMailSender : IMailSender
    {
        private readonly SmtpClient _client;

        /// <summary>
        ///     Creates a new SMTPMailMessage sender based on System.Net.Mail.SmtpClient
        /// </summary>
        public SmtpMailSender() : this(new SmtpClient())
        {
        }

        /// <summary>
        ///     Creates a new SMTPMailMessage sender based on System.Net.Mail.SmtpClient
        /// </summary>
        /// <param name="client">The underlying SmtpClient instance to use.</param>
        public SmtpMailSender(SmtpClient client)
        {
            _client = client;
        }


        /// <summary>
        ///     Sends SMTPMailMessage synchronously.
        /// </summary>
        /// <param name="mailAttributes">The SmtpMailAttributes you wish to send.</param>
        public void Send(IMailAttributes mailAttributes)
        {
            MailMessage mail = ((SmtpMailAttributes) mailAttributes).GenerateProspectiveMailMessage();
            _client.Send(mail);
        }

        /// <summary>
        ///     Sends SMTPMailMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The IMailAttributes message you wish to send.</param>
        public async Task<IMailAttributes> SendAsync(IMailAttributes mailAttributes)
        {
            MailMessage mail = ((SmtpMailAttributes) mailAttributes).GenerateProspectiveMailMessage();
            await _client.SendMailAsync(mail);
            return mailAttributes;
        }

        /// <summary>
        ///     Destroys the underlying SmtpClient.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}