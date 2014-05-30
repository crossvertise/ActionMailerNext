using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ActionMailer.Net.Mvc {
    /// <summary>
    /// The base class that your controller should inherit from if you wish
    /// to send emails through ActionMailer.Net.
    /// </summary>
    public abstract class MailerBase : Controller, IMailerBase {
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
        public IList<string> To { get; private set; }

        /// <summary>
        /// A collection of addresses that should be CC'ed.
        /// </summary>
        public IList<string> CC { get; private set; }

        /// <summary>
        /// A collection of addresses that should be BCC'ed.
        /// </summary>
        public IList<string> BCC { get; private set; }

        /// <summary>
        /// A collection of addresses that should be listed in Reply-To header.
        /// </summary>
        public IList<string> ReplyTo { get; private set; }

        /// <summary>
        /// Any custom headers (name and value) that should be placed on the message.
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Gets or sets the default message encoding when delivering mail.
        /// </summary>
        public Encoding MessageEncoding { get; set; }

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
        /// The underlying IMailSender to use for outgoing messages.
        /// </summary>
        public IMailSender MailSender { get; set; }

        /// <summary>
        /// Gets or sets the http context to use when constructing EmailResult's.
        /// </summary>
        public HttpContextBase HttpContextBase { get; set; }

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
        /// Initializes MailerBase using the default SmtpMailSender and system Encoding.
        /// </summary>
        /// <param name="mailSender">The underlying mail sender to use for delivering mail.</param>
        /// <param name="defaultMessageEncoding">The default encoding to use when generating a mail message.</param>
        protected MailerBase(IMailSender mailSender = null, Encoding defaultMessageEncoding = null) {
            From = null;
            Subject = null;
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
            ReplyTo = new List<string>();
            Headers = new Dictionary<string, string>();
            Attachments = new AttachmentCollection();
            MailSender = mailSender ?? new SmtpMailSender();
            MessageEncoding = defaultMessageEncoding ?? Encoding.UTF8;

            if (System.Web.HttpContext.Current != null) {
                HttpContextBase = new HttpContextWrapper(System.Web.HttpContext.Current);
                var routeData = RouteTable.Routes.GetRouteData(HttpContextBase) ?? new RouteData();
                var requestContext = new RequestContext(HttpContextBase, routeData);
                base.Initialize(requestContext);
            }
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="model">The model object used while rendering the message body.</param>
        /// <param name="masterName">The master page to use when rendering the message body.</param>
        /// <param name="trimBody">Whether or not we should trim whitespace from the beginning and end of the message body.</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public virtual EmailResult Email(string viewName, object model = null, string masterName = null, bool trimBody = true) {
            if (viewName == null)
                throw new ArgumentNullException("viewName");

            var mail = this.GenerateMail();
            var result = new EmailResult(this, MailSender, mail, viewName, masterName, MessageEncoding, trimBody);
            ViewData.Model = model;
            result.ViewData = ViewData;

            var routeData = new RouteData();
            routeData.DataTokens["area"] = FindAreaName();
            routeData.Values["controller"] = GetType().Name.Replace("Controller", string.Empty);
            routeData.Values["action"] = viewName;

            var requestContext = new RequestContext(HttpContextBase, routeData);
            ControllerContext = new ControllerContext(requestContext, this);

            result.ExecuteResult(ControllerContext);
            return result;
        }

        private string FindAreaName() {
            string area = null;

            if (HttpContextBase != null &&
                HttpContextBase.Request != null &&
                HttpContextBase.Request.RequestContext != null &&
                HttpContextBase.Request.RequestContext.RouteData != null) {

                    if (HttpContextBase.Request.RequestContext.RouteData.DataTokens.ContainsKey("area")) {
                        area = HttpContextBase.Request.RequestContext.RouteData.DataTokens["area"].ToString();
                    }
            }

            if (area == null) {
                var name = GetType().Namespace;
                if (name != null && name.Contains(".Areas.")) {
                    var startIndex = name.IndexOf(".Areas.", StringComparison.Ordinal) + 7;
                    var length = name.LastIndexOf(".", StringComparison.Ordinal) - startIndex;
                    area = name.Substring(startIndex, length);
                }
            }

            return area;
        }

        /// <summary>
        /// Nothing to do here, left empty for now.
        /// </summary>
        protected override void ExecuteCore() { }

        /// <summary>
        /// Dispose of the underlying MailSender when this controller is destroyed.
        /// </summary>
        /// <param name="disposing">Whether we are disposing or not.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (MailSender != null) {
                    MailSender.Dispose();
                    MailSender = null;
                }                
            }

            base.Dispose(disposing);
        }
    }
}