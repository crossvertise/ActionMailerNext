using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace ActionMailer.Net.Standalone {
    /// <summary>
    /// This is a standalone MailerBase that relies on RazorEngine to generate emails.
    /// </summary>
    public abstract class RazorMailerBase : IMailerBase {
        /// <summary>
        /// The path to the folder containing your Razor views.
        /// </summary>
        public abstract string ViewPath { get; }

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
        /// A template resolver that is used to find the appropriate templates
        /// </summary>
        public ITemplateResolver TemplateResolver { get; set; }

        /// <summary>
        /// A template base that can add more features to RazorEngine
        /// </summary>
        public Type TemplateBaseType { get; set; }

        /// <summary>
        /// The ViewBag that can be used to pass information to the views
        /// </summary>
        public DynamicViewBag ViewBag { get; set; }

        private ITemplateService _templateService;
        private ITemplateService TemplateService {
            get {
                if (_templateService == null) {
                    var config = new TemplateServiceConfiguration {
                        BaseTemplateType = TemplateBaseType ?? typeof(TemplateBase<>),
                        Resolver = TemplateResolver ?? new RazorTemplateResolver(ViewPath),
                    };

                    _templateService = new TemplateService(config);
                }
                return _templateService;
            }
        }



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
        protected RazorMailerBase(IMailSender mailSender = null, Encoding defaultMessageEncoding = null) {
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
            ViewBag = new DynamicViewBag();
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="trimBody">Whether or not we should trim whitespace from the beginning and end of the message body.</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public virtual RazorEmailResult Email(string viewName, bool trimBody = true) {
            return Email<object>(viewName, null, trimBody);
        }

        /// <summary>
        /// Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="model">The model object used while rendering the message body.</param>
        /// <param name="trimBody">Whether or not we should trim whitespace from the beginning and end of the message body.</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public virtual RazorEmailResult Email<T>(string viewName, T model = null, bool trimBody = true) where T : class {
            if (viewName == null)
                throw new ArgumentNullException("viewName");

            var mail = this.GenerateMail();
            var result = new RazorEmailResult(this, MailSender, mail, viewName, MessageEncoding, ViewPath, TemplateService, ViewBag);

            result.Compile(model, trimBody);
            return result;
        }
    }
}