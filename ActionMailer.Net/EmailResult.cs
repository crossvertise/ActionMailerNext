﻿#region License
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
        private readonly object _model;
        private readonly IMailInterceptor _interceptor;
        private readonly IMailSender _mailSender;

        /// <summary>
        /// The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        public readonly MailMessage Mail;

        /// <summary>
        /// Creates a new EmailResult.  You must call ExecuteCore() before this result
        /// can be successfully delivered.
        /// </summary>
        /// <param name="interceptor">The IMailInterceptor that we will call when delivering mail.</param>
        /// <param name="sender">The IMailSender that we will use to send mail.</param>
        /// <param name="mail">The mail message who's body needs populating.</param>
        /// <param name="viewName">The view to use when rendering the message body (can be null)</param>
        /// <param name="masterName">The maste rpage to use when rendering the message body (can be null)</param>
        /// <param name="model">The model object to pass to the view when rendering the message body (can be null)</param>
        public EmailResult(IMailInterceptor interceptor, IMailSender sender, MailMessage mail, string viewName, string masterName, object model) {
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            if (sender == null)
                throw new ArgumentNullException("sender");

            if (mail == null)
                throw new ArgumentNullException("message");

            ViewName = viewName ?? ViewName;
            MasterName = masterName ?? MasterName;
            Mail = mail;
            _mailSender = sender;
            _model = model;
            _interceptor = interceptor;
        }

        /// <summary>
        /// Causes the body of the mail message to be generated.
        /// </summary>
        /// <param name="context">The controller context to use while rendering the body.</param>
        public override void ExecuteResult(ControllerContext context) {
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

        private void Deliver(MailMessage mail, bool async) {
            var mailContext = new MailSendingContext(mail);
            _interceptor.OnMailSending(mailContext);

            if (mailContext.Cancel) {
                return;
            }

            using (_mailSender) {
                if (async) {
                    _mailSender.SendAsync(mail, AsyncSendCompleted);
                    return; // prevent the OnMailSent() method from being called too early.
                }
                else {
                    _mailSender.Send(mail);
                }
            }

            _interceptor.OnMailSent(mail);
        }

        private void AsyncSendCompleted(MailMessage mail) {
            _interceptor.OnMailSent(mail);
        }

        private string RenderViewToString(ControllerContext context) {
            if (string.IsNullOrEmpty(ViewName))
                ViewName = context.RouteData.GetRequiredString("action");

            ViewData.Model = _model;

            using (var writer = new StringWriter()) {
                if (View == null)
                    View = FindView(context).View;

                var viewContext = new ViewContext(context, View, ViewData, TempData, writer);
                View.Render(viewContext, writer);
                return writer.GetStringBuilder().ToString().Trim();
            }
        }
    }
}