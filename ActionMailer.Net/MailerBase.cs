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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ActionMailer.Net {
    /// <summary>
    /// The base class that your controller should inherit from if you wish
    /// to send emails through ActionMailer.Net.
    /// </summary>
    public class MailerBase : ControllerBase, IMailInterceptor {
        /// <summary>
        /// A string representation of who this mail should be from.  Could be
        /// your name and email address or just an email address by itself.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// The subject line of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// A collection of addresses this email should be sent to.
        /// </summary>
        public List<string> To { get; private set; }

        /// <summary>
        /// A collection of addresses that should be CC'ed.
        /// </summary>
        public List<string> CC { get; private set; }

        /// <summary>
        /// A collection of addresses that should be BCC'ed.
        /// </summary>
        public List<string> BCC { get; private set; }

        /// <summary>
        /// A collection of addresses that should be listed in Reply-To header.
        /// </summary>
        public List<string> ReplyTo { get; private set; }

        /// <summary>
        /// Any custom headers (name and value) that should be placed on the message.
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Any attachments you wish to add.  The key of this collection is what
        /// the file should be named.  The value is should represent the binary bytes
        /// of the file.
        /// </summary>
        /// <example>
        /// Attachments["picture.jpg"] = File.ReadAllBytes(@"C:\picture.jpg");
        /// </example>
        public AttachmentCollection Attachments { get; private set; }

        /// <summary>
        /// Gets or sets the http context to use when constructing EmailResult's.
        /// </summary>
        public HttpContextBase HttpContextBase { get; set; }

        /// <summary>
        /// The underlying IMailSender to use for outgoing messages.
        /// </summary>
        public IMailSender MailSender { get; set; }

        /// <summary>
        /// This method is called after each mail is sent.
        /// </summary>
        /// <param name="mail">The mail that was sent.</param>
        protected virtual void OnMailSent(MailMessage mail) { }

        /// <summary>
        /// This method is called before each mail is sent
        /// </summary>
        /// <param name="context">A simple context containing the mail
        /// and a boolean value that can be toggled to prevent this
        /// mail from being sent.</param>
        protected virtual void OnMailSending(MailSendingContext context) { }

        void IMailInterceptor.OnMailSending(MailSendingContext context) {
            OnMailSending(context);
        }

        void IMailInterceptor.OnMailSent(MailMessage mail) {
            OnMailSent(mail);
        }

        /// <summary>
        /// Initializes MailerBase using the default SmtpMailSender.
        /// </summary>
        /// <param name="mailSender">The underlying mail sender to use for delivering mail.</param>
        public MailerBase(IMailSender mailSender = null) {
            From = null;
            Subject = null;
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
            ReplyTo = new List<string>();
            Headers = new Dictionary<string, string>();
            Attachments = new AttachmentCollection();
            MailSender = mailSender ?? new SmtpMailSender();
            if (HttpContext.Current != null) {
                HttpContextBase = new HttpContextWrapper(HttpContext.Current);
            }
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
            var mail = GenerateMail();
            var sender = new SmtpMailSender();
            var result = new EmailResult(this, sender, mail, viewName, masterName, model);

            var routeData = new RouteData();
            routeData.Values["controller"] = this.GetType().Name.Replace("Controller", string.Empty);
            routeData.Values["action"] = FindActionName();

            var requestContext = new RequestContext(HttpContextBase, routeData);
            var context = new ControllerContext(requestContext, this);

            result.ExecuteResult(context);
            return result;
        }

        // TODO:  Is there a better way to do this?  It feels dirty... Maybe
        //        check MVC3's source and see how they do it.
        private string FindActionName() {
            // since the stack trace is a "stack" we can work our way up the stack
            // until we find a method that isn't named "Email."  The first method
            // we encounter without that name *should* be our action.
            string action = null;
            var trace = new StackTrace();
            // start at 1, since 0 will be this method.
            for (int i = 1; i < trace.FrameCount; i++) {
                int counter = i;
                var methodName = trace.GetFrame(counter).GetMethod().Name;
                if (methodName != "Email") {
                    action = methodName;
                    break;
                }
            }

            return action;
        }

        private MailMessage GenerateMail() {
            var message = new MailMessage();
            To.ForEach(x => message.To.Add(new MailAddress(x)));
            CC.ForEach(x => message.CC.Add(new MailAddress(x)));
            BCC.ForEach(x => message.Bcc.Add(new MailAddress(x)));
            ReplyTo.ForEach(x => message.ReplyToList.Add(new MailAddress(x)));
            message.From = new MailAddress(From);
            message.Subject = Subject;

            foreach (var kvp in Headers)
                message.Headers[kvp.Key] = kvp.Value;

            foreach (var kvp in Attachments)
                message.Attachments.Add(CreateAttachment(kvp.Key, kvp.Value, false));

            foreach (var kvp in Attachments.Inline)
                message.Attachments.Add(CreateAttachment(kvp.Key, kvp.Value, true));

            return message;
        }

        private Attachment CreateAttachment(string fileName, byte[] fileContents, bool inline) {
            // ideally we'd like to find the mime type for each attachment automatically
            // based on the file extension.
            string mimeType = null;
            var extension = fileName.Substring(fileName.LastIndexOf("."));
            if (!string.IsNullOrEmpty(extension))
                mimeType = MimeTypes.ResolveByExtension(extension);

            var stream = new MemoryStream(fileContents);
            var attachment = new Attachment(stream, fileName, mimeType);
            attachment.ContentDisposition.Inline = inline;
            return attachment;
        }

        /// <summary>
        /// Nothing to do here, left empty for now.
        /// </summary>
        protected override void ExecuteCore() { }
    }
}