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
using System.Text;
using System.Net.Mime;

namespace ActionMailer.Net {
    /// <summary>
    /// A special result that should be returned from each eaction in your
    /// mail controller.  Your controller must inherit from MailerBase.
    /// </summary>
    public class EmailResult : ViewResult {
        private readonly object _model;
        private readonly IMailInterceptor _interceptor;
        private readonly IMailSender _mailSender;

        private IView _htmlView;
        private string _htmlViewName;

        private IView _textView;
        private string _textViewName;

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
                throw new ArgumentNullException("mail");

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
            AddMessageViews(context);
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

        private void LocateViews(ControllerContext context) {
            if (context == null)
                throw new ArgumentNullException("context");

            if (string.IsNullOrEmpty(ViewName))
                ViewName = context.RouteData.GetRequiredString("action");

            _htmlViewName = String.Format("{0}.html", ViewName);
            _textViewName = String.Format("{0}.txt", ViewName);

            var htmlViewResult = ViewEngines.Engines.FindView(context, _htmlViewName, MasterName);
            if (htmlViewResult.View != null) {
                _htmlView = htmlViewResult.View;
            }

            var textViewResult = ViewEngines.Engines.FindView(context, _textViewName, MasterName);
            if (textViewResult.View != null) {
                _textView = textViewResult.View;
            }
        }

        private string RenderViewAsString(ControllerContext context, IView view) {
            using (var writer = new StringWriter()) {
                var viewContext = new ViewContext(context, view, ViewData, TempData, writer);
                view.Render(viewContext, writer);
                return writer.GetStringBuilder().ToString();
            }
        }

        private void AddMessageViews(ControllerContext context) {
            if (context == null)
                throw new ArgumentNullException("context");

            LocateViews(context);

            if (_textView == null && _htmlView == null) {
                var message = String.Format("You must provide a view for this email.  Views should be named {0}.text.cshtml or {1}.html.cshtml (or aspx for WebFormsViewEngine) depending on the format you wish to render.", ViewName, ViewName);
                throw new NoViewsFoundException(message);
            }

            var multiPart = false;
            if (_textView != null && _htmlView != null)
                multiPart = true;

            ViewData.Model = _model;

            if (_htmlView != null) {
                var body = RenderViewAsString(context, _htmlView);

                if (multiPart) {
                    var altView = AlternateView.CreateAlternateViewFromString(body, Encoding.Default, MediaTypeNames.Text.Html);
                    Mail.AlternateViews.Add(altView);
                }
                else {
                    Mail.IsBodyHtml = true;
                    Mail.Body = body;
                }
            }

            if (_textView != null) {
                var body = RenderViewAsString(context, _textView);

                if (multiPart) {
                    var altView = AlternateView.CreateAlternateViewFromString(body, Encoding.Default, MediaTypeNames.Text.Plain);
                    Mail.AlternateViews.Add(altView);
                }
                else {
                    Mail.Body = body;
                }
            }
        }
    }
}