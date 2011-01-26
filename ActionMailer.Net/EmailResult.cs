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
using System.IO;
using System.Net.Mail;
using System.Web.Mvc;
using System.ComponentModel;

namespace ActionMailer.Net {
    /// <summary>
    /// A special result that should be returned from each eaction in your
    /// mail controller.  Your controller must inherit from MailerBase.
    /// </summary>
    public class EmailResult : ViewResult {
        /// <summary>
        /// The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        public readonly MailMessage Mail;
        private readonly object _model;
        private MailerBase _mailer;

        /// <summary>
        /// Creates a new EmailResult.  You must call ExecuteCore() before this result
        /// can be successfully delivered.
        /// </summary>
        /// <param name="mail">The mail message who's body needs populating.</param>
        /// <param name="viewName">The view to use when rendering the message body (can be null)</param>
        /// <param name="masterName">The maste rpage to use when rendering the message body (can be null)</param>
        /// <param name="model">The model object to pass to the view when rendering the message body (can be null)</param>
        public EmailResult(MailMessage mail, string viewName, string masterName, object model) {
            if (mail == null)
                throw new ArgumentNullException("message");

            ViewName = viewName ?? ViewName;
            MasterName = masterName ?? MasterName;
            Mail = mail;
            _model = model;
        }

        /// <summary>
        /// Causes the body of the mail message to be generated.
        /// </summary>
        /// <param name="context">The controller context to use while rendering the body.</param>
        public override void ExecuteResult(ControllerContext context) {
            _mailer = context.Controller as MailerBase;
            if (_mailer == null)
                throw new ArgumentException("If you want to use the Email helper, your controller must inherit from MailerBase.");

            Mail.Body = RenderViewToString(context);
        }

        /// <summary>
        /// Sends your message.  This call will block while the message is being sent. (not recommended)
        /// </summary>
        public void Deliver() {
            Deliver(Mail, false);
        }

        /// <summary>
        /// Sends your message asynchronously.  This method does not block.  If you need to know
        /// when the message has been sent, then override the OnMailSent method in MailerBase which
        /// will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public void DeliverAsync() {
            Deliver(Mail, true);
        }

        internal void Deliver(MailMessage mail, bool async) {
            var interceptor = (IMailInterceptor)_mailer;
            var mailContext = new MailSendingContext(mail);
            interceptor.OnMailSending(mailContext);

            if (mailContext.Cancel) {
                return;
            }

            using (var client = new SmtpClient()) {
                if (async) {
                    client.SendCompleted += new SendCompletedEventHandler(AsyncSendCompleted);
                    client.SendAsync(mail, mail);
                    return; // prevent the OnMailSent() method from being called too early.
                }
                else {
                    client.Send(mail);
                }
            }

            interceptor.OnMailSent(mail);
        }

        private void AsyncSendCompleted(object sender, AsyncCompletedEventArgs e) {
            var interceptor = (IMailInterceptor)_mailer;
            interceptor.OnMailSent(e.UserState as MailMessage);
        }

        private string RenderViewToString(ControllerContext context) {
            if (string.IsNullOrEmpty(ViewName))
                ViewName = context.RouteData.GetRequiredString("action");

            ViewData.Model = _model;

            using (var writer = new StringWriter()) {
                var viewResult = ViewEngines.Engines.FindView(context, ViewName, MasterName);
                var viewContext = new ViewContext(context, viewResult.View, ViewData, TempData, writer);
                viewResult.View.Render(viewContext, writer);
                return writer.GetStringBuilder().ToString().Trim();
            }
        }
    }
}