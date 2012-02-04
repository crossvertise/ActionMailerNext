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
using System.Net.Mail;

namespace ActionMailer.Net {
    /// <summary>
    /// Implements IMailSender by using System.Net.Mail.SmtpClient.
    /// </summary>
    public class SmtpMailSender : IMailSender {
        private readonly SmtpClient _client;
        private Action<MailMessage> _callback;

        /// <summary>
        /// Creates a new mail sender based on System.Net.Mail.SmtpClient
        /// </summary>
        public SmtpMailSender() : this(new SmtpClient()) { }

        /// <summary>
        /// Creates a new mail sender based on System.Net.Mail.SmtpClient
        /// </summary>
        /// <param name="client">The underlying SmtpClient instance to use.</param>
        public SmtpMailSender(SmtpClient client) {
            _client = client;
        }


        /// <summary>
        /// Sends mail synchronously.
        /// </summary>
        /// <param name="mail">The mail you wish to send.</param>
        public void Send(MailMessage mail) {
            _client.Send(mail);
        }

        /// <summary>
        /// Sends mail asynchronously.
        /// </summary>
        /// <param name="mail">The mail you wish to send.</param>
        /// <param name="callback">The callback method to invoke when the send operation is complete.</param>
        public void SendAsync(MailMessage mail, Action<MailMessage> callback) {
            _callback = callback;
            _client.SendCompleted += new SendCompletedEventHandler(AsyncSendCompleted);
            _client.SendAsync(mail, mail);
        }

        private void AsyncSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            // unsubscribe from the event so _client can be GC'ed if necessary
            _client.SendCompleted -= AsyncSendCompleted;
            _callback(e.UserState as MailMessage);
        }

        /// <summary>
        /// Destroys the underlying SmtpClient.
        /// </summary>
        public void Dispose() {
            _client.Dispose();
        }
    }
}
