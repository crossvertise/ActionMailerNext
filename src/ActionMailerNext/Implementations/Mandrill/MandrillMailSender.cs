using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        public MandrillMailSender() : this(ConfigurationManager.AppSettings["MandrillApiKey"]){}

        /// <summary>
        ///     Creates a new MandrillApi based onMandrill.MandrillApi
        /// </summary>
        /// <param name="apiKey"></param>
        public MandrillMailSender(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException("apiKey",
                    "The AppSetting 'MandrillApiKey' is not defined. Either define this configuration section or use the constructor with apiKey parameter.");

            _client = new MandrillApi(apiKey);
        }


        /// <summary>
        ///     Sends MandrillMessage synchronously.
        /// </summary>
        /// <param name="mailAttributes">The IMailAttributes you wish to send.</param>
        public List<IMailResponse> Send(IMailAttributes mailAttributes)
        {
            var mail = ((MandrillMailAttributes) mailAttributes).GenerateProspectiveMailMessage();
            var response = new List<IMailResponse>();

            var re = _client.SendMessage(mail);
            response.AddRange(re.Select(result => new MandrillMailResponse
            {
                Email = result.Email, Status = result.Status.ToString(), RejectReason = result.RejectReason, Id = result.Id
            }));

            return response;
        }

        /// <summary>
        ///     Sends MandrillMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The IMailAttributes message you wish to send.</param>
        public async Task<List<IMailResponse>> SendAsync(IMailAttributes mailAttributes)
        {
            var mail = ((MandrillMailAttributes) mailAttributes).GenerateProspectiveMailMessage();
            var response = new List<IMailResponse>();

            await _client.SendMessageAsync(mail).ContinueWith(x => response.AddRange(x.Result.Select(result => new MandrillMailResponse
            {
                Email = result.Email, Status = result.Status.ToString(), RejectReason = result.RejectReason, Id = result.Id
            })));


            return response;
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