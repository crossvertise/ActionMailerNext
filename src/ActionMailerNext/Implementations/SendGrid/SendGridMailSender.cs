using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;
using SendGrid;

namespace ActionMailerNext.Implementations.SendGrid
{
    /// <summary>
    ///     Implements IMailSender by using SendGrid.Web
    /// </summary>
    public class SendGridMailSender : IMailSender
    {
        private readonly Web _client;

        /// <summary>
        ///     Creates a new SendGridMailSender based on SendGrid.Web
        /// </summary>
        public SendGridMailSender()
        {
            var username = ConfigurationManager.AppSettings["SendGridUser"];
            var password = ConfigurationManager.AppSettings["SendGridPass"];
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(
                    "The AppSetting 'SendGridUser' and 'SendGridPass'  are not defined correctly. Either define this configuration section or use the constructor with username and password parameter.");

            var credentials = new NetworkCredential(username, password);
            _client = new Web(credentials);
        }

        /// <summary>
        ///     Creates a new SendGridMailMessage sender based on SendGridApi
        /// </summary>
        public SendGridMailSender(string username, string password)
        {
            var credentials = new NetworkCredential(username, password);
            _client = new Web(credentials);
        }


        /// <summary>
        ///     Sends SendGridMailMessage synchronously.
        /// </summary>
        /// <param name="mailAttributes">The SendGridMailAttributes you wish to send.</param>
        public List<IMailResponse> Send(IMailAttributes mailAttributes)
        {
            var mail = ((SendGridMailAttributes)mailAttributes).GenerateProspectiveMailMessage();
            _client.Deliver(mail);

            return null;
        }

        /// <summary>
        ///     Sends SendGridMailMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The IMailAttributes message you wish to send.</param>
        public async Task<List<IMailResponse>> SendAsync(IMailAttributes mailAttributes)
        {
            var mail = ((SendGridMailAttributes) mailAttributes).GenerateProspectiveMailMessage();
            await _client.DeliverAsync(mail);

            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            
        }
    }
}