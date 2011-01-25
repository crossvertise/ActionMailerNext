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
using System.Web.Routing;
using System.Diagnostics;
using System.Web;

namespace ActionMailer.Net {
    public class MailerBase : ControllerBase {
        public string From { get; set; }
        public string Subject { get; set; }
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        

        /// <summary>
        /// Event details for the OnMailSent event
        /// </summary>
        /// <param name="sender">The event sender.  Usually &quot;this&quot;</param>
        /// <param name="e">Event arguments including the MailMessage that was sent.</param>
        public delegate void OnMailSentEvent(object sender, MailSentEventArgs e);

        /// <summary>
        /// Event details for the OnMailSending event.  You can prevent messages from
        /// being sent by setting the e.Cancel boolean to &quot;true&quot;
        /// </summary>
        /// <param name="sender">The event sender.  Usually &quot;this&quot;</param>
        /// <param name="e">Event arguments including the MailMessage that is being sent.</param>
        public delegate void OnMailSendingEvent(object sender, MailSendingEventArgs e);

        /// <summary>
        /// This event is called after a message is successfully sent.  If you delivery
        /// the message asynchronously, this event will fire after the asynchonous callback
        /// is received.
        /// </summary>
        public event OnMailSentEvent OnMailSent;

        /// <summary>
        /// This event is called before any messages are delivered.  You can use this
        /// event to inspect the MailMessage before it goes out or to cancel altogether.
        /// </summary>
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
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public EmailResult Email() {
            return Email(null, null, null);
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public EmailResult Email(string viewName) {
            return Email(viewName, null, null);
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="model">The model object used while rendering the message body.</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public EmailResult Email(object model) {
            return Email(null, null, model);
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="model">The model object used while rendering the message body.</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public EmailResult Email(string viewName, object model) {
            return Email(viewName, null, model);
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="masterName">The master page to use when rendering the message body.</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public EmailResult Email(string viewName, string masterName) {
            return Email(viewName, masterName, null);
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="masterName">The master page to use when rendering the message body.</param>
        /// <param name="model">The model object used while rendering the message body.</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public EmailResult Email(string viewName, string masterName, object model) {
            var message = GenerateMailMessage();
            var result = new EmailResult(message, viewName, model);

            var routeData = new RouteData();
            routeData.Values["controller"] = this.GetType().Name.Replace("Controller", string.Empty);

            // since the stack trace is a "stack" we can work our way up the stack
            // until we find a method that isn't named "Email."  The first method
            // we encounter without that name *should* be our action.
            var trace = new StackTrace();
            for (int i = 0; i < trace.FrameCount; i++) {
                int counter = i;
                var methodName = trace.GetFrame(counter).GetMethod().Name;
                if (methodName != "Email") {
                    routeData.Values["action"] = methodName;
                    break;
                }
            }

            var requestContext = new RequestContext(new HttpContextWrapper(HttpContext.Current), routeData);
            var context = new ControllerContext(requestContext, this);

            result.ExecuteResult(context);
            return result;
        }

        private MailMessage GenerateMailMessage() {
            var message = new MailMessage();
            To.ForEach(x => message.To.Add(new MailAddress(x)));
            CC.ForEach(x => message.CC.Add(new MailAddress(x)));
            BCC.ForEach(x => message.Bcc.Add(new MailAddress(x)));
            message.From = new MailAddress(From);
            message.Subject = Subject;

            foreach (var header in Headers) {
                message.Headers.Add(header.Key, header.Value);
            }

            return message;
        }

        /// <summary>
        /// Nothing to do here, left empty for now.
        /// </summary>
        protected override void ExecuteCore() { }
    }
}
