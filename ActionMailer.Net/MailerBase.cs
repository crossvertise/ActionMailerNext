#region License
/* Copyright (C) 2011 by Scott W. Anderson
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Net.Mail;
using System.ComponentModel;

namespace ActionMailer.Net {
    public class MailerBase : ControllerBase {
        public string From { get; set; }
        public string Subject { get; set; }
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public Dictionary<string, byte[]> Attachments { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public delegate void OnMailSentEvent(object sender, MailSentEventArgs e);
        public delegate void OnMailSendingEvent(object sender, MailSendingEventArgs e);
        public event OnMailSentEvent OnMailSent;
        public event OnMailSendingEvent OnMailSending;

        internal void Deliver(MailMessage message, bool async) {
            var sendingArgs = new MailSendingEventArgs(message);
            if (OnMailSending != null) {
                OnMailSending(this, sendingArgs);
            }

            if (sendingArgs.Cancel) {
                return;
            }

            using (var client = new SmtpClient()) {
                if (async) {
                    client.SendCompleted += new SendCompletedEventHandler(AsyncSendCompleted);
                    client.SendAsync(message, message);
                    return;
                }
                else {
                    client.Send(message);
                }
            }

            if (OnMailSent != null) {
                var sentArgs = new MailSentEventArgs(message);
                OnMailSent(this, sentArgs);
            }
        }

        void AsyncSendCompleted(object sender, AsyncCompletedEventArgs e) {
            if (OnMailSent != null) {
                OnMailSent(this, new MailSentEventArgs(e.UserState as MailMessage));
            }
        }

        public MailerBase() {
            From = null;
            Subject = null;
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
            Attachments = new Dictionary<string, byte[]>();
            Headers = new Dictionary<string, string>();
        }

        public EmailResult Email() {
            return Email(null, null, null);
        }

        public EmailResult Email(string viewName) {
            return Email(viewName, null, null);
        }

        public EmailResult Email(object model) {
            return Email(null, null, model);
        }

        public EmailResult Email(string viewName, object model) {
            return Email(viewName, null, model);
        }

        public EmailResult Email(string viewName, string masterName) {
            return Email(viewName, masterName, null);
        }

        public EmailResult Email(string viewName, string masterName, object model) {
            var message = new MailMessage();
            To.ForEach(x => message.To.Add(new MailAddress(x)));
            CC.ForEach(x => message.CC.Add(new MailAddress(x)));
            BCC.ForEach(x => message.Bcc.Add(new MailAddress(x)));
            message.From = new MailAddress(From);
            message.Subject = Subject;

            foreach (var attachment in Attachments) {
            }

            foreach (var header in Headers) {
                message.Headers.Add(header.Key, header.Value);
            }

            var result = new EmailResult(message, viewName, model);
            result.ExecuteResult(ControllerContext);
            return result;
        }

        /// <summary>
        /// Nothing to do here, left empty for now.
        /// </summary>
        protected override void ExecuteCore() {
        }
    }
}
