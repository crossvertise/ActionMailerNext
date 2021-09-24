using System;
using ActionMailerNext.Implementations.SMTP;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Standalone.Helpers;

namespace ActionMailerNext.Standalone
{
    using System.Collections.Generic;
    using System.Dynamic;

    /// <summary>
    ///     This is a standalone MailerBase that relies on RazorEngine to generate emails.
    /// </summary>
    public abstract class HbsMailerBase : IMailInterceptor
    {
        private ITemplateService _templateService;

        private ITemplateResolver _templateResolver;

        private dynamic _viewbag; 

        /// <summary>
        /// 
        /// </summary>
        public MailAttributes MailAttributes { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailAttributes"></param>
        /// <param name="mailSender"></param>
        /// <param name="templateResolver"></param>
        protected HbsMailerBase(MailAttributes mailAttributes = null, IMailSender mailSender = null, ITemplateResolver templateResolver = null, ITemplateService templateService = null)
        {
            MailAttributes = mailAttributes ?? new MailAttributes();
            MailSender = mailSender ?? new SmtpMailSender();
            _templateResolver = templateResolver;
            ViewBag = new ExpandoObject();
            _templateService = templateService;
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
                return this._templateResolver ?? (_templateResolver = new HandlebarsFilesTemplateResolver(GlobalViewPath));
            }
            set
            {
                _templateResolver = value;
            }
        }

        /// <summary>
        ///     Used to add needed variable
        /// </summary>
        public dynamic ViewBag
        {
            get
            {
                return _viewbag;
            }
            set
            {
                _viewbag = value;
            }
        }

        protected ITemplateService TemplateService
        {
            get
            {
                return _templateService ?? (_templateService = new TemplateService(TemplateResolver, ViewSettings));
            }
            set
            {
                _templateService = value;
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
        /// <param name="model">The model object used while rendering the message body.</param>
        /// <param name="masterName">the main layout</param>
        /// <param name="trimBody">Whether or not we should trim whitespace from the beginning and end of the message body.</param>
        /// <param name="externalViewPath">a View path that overrides the one set by the property</param>
        /// <returns>An EmailResult that you can Deliver();</returns>
        public virtual HbsEmailResult Email(string viewName, object model = null, string masterName = null, bool trimBody = true, string externalViewPath = null)
        {
            if (viewName == null)
            {
                throw new ArgumentNullException("viewName");
            }

            if (ViewBag != null)
            {
                ViewBag.ViewSettings = ViewSettings;
            }
                
            var result = new HbsEmailResult(MailAttributes, viewName, MailAttributes.MessageEncoding, masterName,
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
        public virtual void PreCompileViews(List<string> views)
        {
            foreach (var view in views)
            {
                this.TemplateService.AddTemplate(view);
            }
        }
    }
}