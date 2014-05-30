namespace ActionMailer.Net.Mvc5_1 {
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using AttachmentCollection = ActionMailer.Net.AttachmentCollection;

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
            this.OnMailSending(context);
        }

        void IMailInterceptor.OnMailSent(MailMessage mail) {
            this.OnMailSent(mail);
        }

        /// <summary>
        /// Initializes MailerBase using the default SmtpMailSender and system Encoding.
        /// </summary>
        /// <param name="mailSender">The underlying mail sender to use for delivering mail.</param>
        /// <param name="defaultMessageEncoding">The default encoding to use when generating a mail message.</param>
        protected MailerBase(IMailSender mailSender = null, Encoding defaultMessageEncoding = null) {
            this.From = null;
            this.Subject = null;
            this.To = new List<string>();
            this.CC = new List<string>();
            this.BCC = new List<string>();
            this.ReplyTo = new List<string>();
            this.Headers = new Dictionary<string, string>();
            this.Attachments = new AttachmentCollection();
            this.MailSender = mailSender ?? new SmtpMailSender();
            this.MessageEncoding = defaultMessageEncoding ?? Encoding.UTF8;

            if (System.Web.HttpContext.Current != null) {
                this.HttpContextBase = new HttpContextWrapper(System.Web.HttpContext.Current);
                var routeData = RouteTable.Routes.GetRouteData(this.HttpContextBase) ?? new RouteData();
                var requestContext = new RequestContext(this.HttpContextBase, routeData);
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
            var result = new EmailResult(this, this.MailSender, mail, viewName, masterName, this.MessageEncoding, trimBody);
            ViewData.Model = model;
            result.ViewData = ViewData;

            var routeData = new RouteData();
            routeData.DataTokens["area"] = this.FindAreaName();
            routeData.Values["controller"] = this.GetType().Name.Replace("Controller", string.Empty);
            routeData.Values["action"] = viewName;

            var requestContext = new RequestContext(this.HttpContextBase, routeData);
            ControllerContext = new ControllerContext(requestContext, this);

            result.ExecuteResult(ControllerContext);
            return result;
        }

        private string FindAreaName() {
            string area = null;

            if (this.HttpContextBase != null &&
                this.HttpContextBase.Request != null &&
                this.HttpContextBase.Request.RequestContext != null &&
                this.HttpContextBase.Request.RequestContext.RouteData != null) {

                    if (this.HttpContextBase.Request.RequestContext.RouteData.DataTokens.ContainsKey("area")) {
                        area = this.HttpContextBase.Request.RequestContext.RouteData.DataTokens["area"].ToString();
                    }
            }

            if (area == null) {
                var name = this.GetType().Namespace;
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
                if (this.MailSender != null) {
                    this.MailSender.Dispose();
                    this.MailSender = null;
                }                
            }

            base.Dispose(disposing);
        }
    }
}