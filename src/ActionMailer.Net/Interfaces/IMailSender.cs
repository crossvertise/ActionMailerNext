using System;
using System.Threading.Tasks;

namespace ActionMailer.Net.Interfaces
{
    /// <summary>
    ///     An object used to deliver SMTPMailMessage.
    /// </summary>
    public interface IMailSender : IDisposable
    {
        /// <summary>
        ///     Sends IMailAttributes synchronously.
        /// </summary>
        /// <param name="mailAttributes">The SMTPMailMessage message you wish to send.</param>
        void Send(IMailAttributes mailAttributes);


        /// <summary>
        ///     Sends IMailAttributes asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The SMTPMailMessage message you wish to send.</param>
        Task<IMailAttributes> SendAsync(IMailAttributes mailAttributes);
    }
}