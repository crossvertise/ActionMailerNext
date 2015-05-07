using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActionMailerNext.Interfaces
{
    /// <summary>
    ///     An object used to deliver SMTPMailMessage.
    /// </summary>
    public interface IMailSender : IDisposable
    {
        /// <summary>
        ///     Sends MailAttributes synchronously.
        /// </summary>
        /// <param name="mailAttributes">The SMTPMailMessage message you wish to send.</param>
        List<IMailResponse> Send(MailAttributes mailAttributes);


        /// <summary>
        ///     Sends MailAttributes asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The SMTPMailMessage message you wish to send.</param>
        Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes);

        /// <summary>
        /// Method to be called directly after defining MailSender to deliver email synchronously.
        /// </summary>
        /// <param name="emailResult"></param>
        /// <returns></returns>
        List<IMailResponse> Deliver(IEmailResult emailResult);

        /// <summary>
        /// Method to be called directly after defining MailSender to deliver email asynchronously.
        /// </summary>
        /// <param name="emailResult"></param>
        /// <returns></returns>
        Task<MailAttributes> DeliverAsync(IEmailResult emailResult);
    }
}