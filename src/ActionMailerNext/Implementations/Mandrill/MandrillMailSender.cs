using System;
using System.Configuration;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;
using Mandrill;

namespace ActionMailerNext.Implementations.Mandrill
{
    /// <summary>
    ///     Implements IMailSender by using Mandrill.MandrillApi
    /// </summary>
    public class MandrillMailSender : IMailSender
    {
        private MandrillApi _client;

        /// <summary>
        ///     Creates a new MandrillMailSender based on Mandrill.MandrillApi
        /// </summary>
        public MandrillMailSender()
        {
            var apiKey = ConfigurationManager.AppSettings["MandrillApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "The AppSetting 'MandrillApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            _client = new MandrillApi(apiKey);
        }

        /// <summary>
        ///     Creates a new MandrillApi based onMandrill.MandrillApi
        /// </summary>
        /// <param name="apiKey"></param>
        public MandrillMailSender(string apiKey)
        {
            _client = new MandrillApi(apiKey);
        }


        /// <summary>
        ///     Sends MandrillMessage synchronously.
        /// </summary>
        /// <param name="mailAttributes">The IMailAttributes you wish to send.</param>
        public void Send(IMailAttributes mailAttributes)
        {
            var mail = ((MandrillMailAttributes) mailAttributes).GenerateProspectiveMailMessage();
            _client.SendMessage(mail);
        }

        /// <summary>
        ///     Sends MandrillMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The IMailAttributes message you wish to send.</param>
        public async Task<IMailAttributes> SendAsync(IMailAttributes mailAttributes)
        {
            var mail = ((MandrillMailAttributes) mailAttributes).GenerateProspectiveMailMessage();

            await _client.SendMessageAsync(mail);
            return mailAttributes;
        }


        /// <summary>
        ///     Destroys the underlying MandrillApi.
        /// </summary>
        public void Dispose()
        {
            _client = null;
        }
    }
}