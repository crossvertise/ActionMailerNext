using System;
using ActionMailerNext.Implementations.SMTP;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Standalone.Helpers;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace ActionMailerNext.Standalone
{
    using System.Collections.Generic;

    /// <summary>
    ///     This is a standalone MailerBase that relies on RazorEngine to generate emails.
    /// </summary>
    public abstract class RazorMailerBase : IMailInterceptor
    {
        /// <summary>
        /// </summary>
        public MailAttributes MailAttributes { get;  set; }

        
        /// <summary>
        /// We use a singleton instance of the templating service in the mailer, to facilitate 
        /// caching of resolved and compiled views.
        /// </summary>
        private static ITemplateService templateService;

        private ITemplateResolver templateResolver;

        /// <summary>
        ///     Initializes MailerBase using the default SmtpMailSender and system Encoding.
        /// </summary>
        /// <param name="mailAttributes"></param>
        /// <param name="mailSender">The underlying mail sender to use for delivering mail.</param>
        protected RazorMailerBase(MailAttributes mailAttributes = null, IMailSender mailSender = null)
        {
            MailAttributes = mailAttributes ?? new MailAttributes();
            MailSender = mailSender ?? new SmtpMailSender();

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
        public ITemplateResolver TemplateResolver
        {
            get
            {
                return this.templateResolver ?? (this.templateResolver = new ExtendedTemplateResolver(GlobalViewPath));
            }
            set
            {
                this.templateResolver = value;
            }
        }

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
                if (templateService == null)
                {
                    var config = new TemplateServiceConfiguration
                    {
                        BaseTemplateType = TemplateBaseType ?? typeof(ExtendedTemplateBase<>),
                        Resolver = TemplateResolver,
                    };

                    templateService = new TemplateService(config);
                    
                }
                return templateService;
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
        void IMailInterceptor.OnMailSent(MailAttributes mail)
        {
            OnMailSent(mail);
        }

        /// <summary>
        ///     This method is called when onMailsent is fired.
        /// </summary>
        /// <param name="mail"></param>
        public void OnMailSent(MailAttributes mail)
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

            var result = new RazorEmailResult(MailAttributes, viewName, MailAttributes.MessageEncoding, masterName,
                externalViewPath ?? GlobalViewPath, TemplateService, ViewBag);

            result.Compile(model, trimBody);

            foreach (var postprocessor in MailAttributes.PostProcessors)
            {
                result.MailAttributes = postprocessor.Execute(result.MailAttributes);
            }

            return result;
        }

        /// <summary>
        /// Pre-Compiles the views in the dictionary using a the template resolver
        /// </summary>
        /// <param name="views"></param>
        public virtual void PreCompileViews(IDictionary<string, Type> views)
        {
            foreach (var view in views)
            {
                var viewName = view.Key;
                var modelType = view.Value;

                var template = this.TemplateResolver.Resolve(viewName);
                this.TemplateService.Compile(template, modelType, viewName);
            }
        }
    }
}