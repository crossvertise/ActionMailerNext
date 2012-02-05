#region License
/* Copyright (C) 2012 by Scott W. Anderson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using RestSharp;

namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// Implementation of IMailSender that supports sending mail through Postmark.
    /// </summary>
    public class PostmarkMailSender : IMailSender {
        private string _serverToken;
        private RestClient _client;

        /// <summary>
        /// Creates a new instance of the Postmark IMailSender implementation.
        /// </summary>
        /// <param name="serverToken">Your Postmark API server token to be used for sending emails.</param>
        public PostmarkMailSender(string serverToken) {
            _serverToken = serverToken;
            _client = new RestClient("http://api.postmarkapp.com");
            _client.AddDefaultHeader("X-Postmark-Server-Token", serverToken);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose() {
            _client = null;
        }

        /// <summary>
        /// Sends the message synchronously through the Postmark API.
        /// </summary>
        /// <param name="mail">The mail message to send.</param>
        public void Send(MailMessage mail) {
            
        }

        /// <summary>
        /// Sends the message asynchronously through the Postmark API and calls
        /// the given callback when complete.
        /// </summary>
        /// <param name="mail">The mail message to send.</param>
        /// <param name="callback">The callback to execute when sending is complete.</param>
        public void SendAsync(MailMessage mail, Action<MailMessage> callback) {
            
        }
    }
}
