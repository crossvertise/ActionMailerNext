using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionMailer.Net.Mandrill
{
    public class MandrillException : Exception
    {
        /// <summary>
        /// The actual response received from the Mandrill API.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Creates a new MandrillException that wraps the given response.
        /// </summary>
        /// <param name="response">The response received from Mandrill.</param>
        public MandrillException(string response) : base(response) {
            
        }
    }
}
