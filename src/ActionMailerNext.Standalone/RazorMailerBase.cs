using System;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Standalone.Helpers;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace ActionMailerNext.Standalone
{
    /// <summary>
    ///     This is a standalone MailerBase that relies on RazorEngine to generate emails.
    /// </summary>
    public abstract class RazorMailerBase : IMailInterceptor
    {
        /// <summary>
        /// </summary>
        public IMailAttributes MailAttributes;

        private ITemplateService _templateService;

        private string _usedViewPath;

        /// <summary>
        ///     Initializes MailerBase using the default SmtpMailSender and system Encoding.
        /// </summary>
        /// <param name="mailAttributes"></param>
        /// <param name="mailSender">The underlying mail sender to use for delivering mail.</param>
        protected RazorMailerBase(IMailAttributes mailAttributes = null, IMailSender mailSender = null)
        {
            MailAttributes = mailAttributes ?? MailSendorFactory.GetAttributes();
            MailSender = mailSender ?? MailSendorFactory.GetSender();

            ViewBag = new DynamicViewBag();
        }

        /// <summary>
        ///     The path to the folder containing your Razor views.
        /// </summary>
        public abstract string GlobalViewPath { get; }

        /// <summary>
        ///     The view settings needed to implement HTML/URL Helpers
        /// </summary>
        public abstract ViewSettings ViewSettings { get;}

        /// <summary>
        ///     The underlying IMailSender to use for outgoing messages.
        /// </summary>
        public IMailSender MailSender { get; set; }

        /// <summary>
        ///     A template resolver that is used to find the appropriate templates
        /// </summary>
        public ITemplateResolver TemplateResolver { get; set; }

        /// <summary>
        ///     A template base that can add more features to RazorEngine
        /// </summary>
        public Type TemplateBaseType { get; set; }

        /// <summary>
        ///     Used to add needed variable
        /// </summary>
        public dynamic ViewBag { get; set; }


        private ITemplateService TemplateService
        {
            get
            {
                if (_templateService == null)
                {
                    var config = new TemplateServiceConfiguration
                    {
                        BaseTemplateType = TemplateBaseType ?? typeof(ExtendedTemplateBase<>),
                        Resolver = TemplateResolver ?? new ExtendedTemplateResolver(_usedViewPath),
                    };

                    _templateService = new TemplateService(config);
                    
                }
                return _templateService;
            }
        }


        /// <summary>
        ///     This method is called before each mail is sent
        /// </summary>
        /// <param name="context">
        ///     A simple context containing the mail
        ///     and a boolean value that can be toggled to prevent this
        ///     mail from being sent.
        /// </param>
        void IMailInterceptor.OnMailSending(MailSendingContext context)
        {
        }

        /// <summary>
        ///     This method is called after each mail is sent.
        /// </summary>
        /// <param name="mail">The mail that was sent.</param>
        void IMailInterceptor.OnMailSent(IMailAttributes mail)
        {
            OnMailSent(mail);
        }

        /// <summary>
        ///     This method is called when onMailsent is fired.
        /// </summary>
        /// <param name="mail"></param>
        public void OnMailSent(IMailAttributes mail)
        {
        }

        /// <summary>
        ///     Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="masterName">the main layout</param>
        /// <param name="trimBody">Whether or not we should trim whitespace from the beginning and end of the message body.</param>
        /// <param name="externalViewPath">a View path that overrides the one set by the property</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public virtual RazorEmailResult Email(string viewName, string masterName = null, bool trimBody = true, string externalViewPath = null)
        {
            return Email<object>(viewName, null, masterName, trimBody);
        }

        /// <summary>
        ///     Constructs your mail message ready for delivery.
        /// </summary>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="model">The model object used while rendering the message body.</param>
        /// <param name="masterName">the main layout</param>
        /// <param name="trimBody">Whether or not we should trim whitespace from the beginning and end of the message body.</param>
        /// <param name="externalViewPath">a View path that overrides the one set by the property</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public virtual RazorEmailResult Email<T>(string viewName, T model = null, string masterName = null, bool trimBody = true, string externalViewPath = null) where T : class
        {
            if (viewName == null)
                throw new ArgumentNullException("viewName");
            if (ViewBag != null)
            {
                ViewBag.ViewSettings = ViewSettings;
            }

            _usedViewPath = externalViewPath ?? GlobalViewPath;
            var result = new RazorEmailResult(this, MailSender, MailAttributes, viewName, MailAttributes.MessageEncoding, masterName,
                _usedViewPath, TemplateService, ViewBag);

            result.Compile(model, trimBody);
            return result;
        }
    }

}