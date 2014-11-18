using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ActionMailerNext.Implementations.SMTP;
using ActionMailerNext.Interfaces;

namespace ActionMailerNext.Mvc5_1
{
    /// <summary>
    ///     The base class that your controller should inherit from if you wish
    ///     to send emails through ActionMailer.Net.
    /// </summary>
    public abstract class MailerBase : Controller, IMailInterceptor
    {
        public MailAttributes MailAttributes;

        /// <summary>
        ///     Initializes MailerBase using the defaultMailSender and system Encoding.
        /// </summary>
        /// <param name="mailAttributes"> the mail attributes</param>
        /// <param name="mailSender">The underlying mail sender to use for delivering mail.</param>
        protected MailerBase(MailAttributes mailAttributes = null, IMailSender mailSender = null)
        {
            MailAttributes = mailAttributes ?? new MailAttributes();
            MailSender = mailSender ?? new SmtpMailSender();

            if (System.Web.HttpContext.Current == null) return;
            HttpContextBase = new HttpContextWrapper(System.Web.HttpContext.Current);
            RouteData routeData = RouteTable.Routes.GetRouteData(HttpContextBase) ?? new RouteData();
            var requestContext = new RequestContext(HttpContextBase, routeData);
            base.Initialize(requestContext);
        }

        /// <summary>
        ///     The underlying IMailSender to use for outgoing messages.
        /// </summary>
        public IMailSender MailSender { get; set; }

        /// <summary>
        ///     Gets or sets the http context to use when constructing EmailResult's.
        /// </summary>
        public HttpContextBase HttpContextBase { get; set; }

        void IMailInterceptor.OnMailSending(MailSendingContext context)
        {
            OnMailSending(context);
        }

        void IMailInterceptor.OnMailSent(MailAttributes mail)
        {
            OnMailSent(mail);
        }

        /// <summary>
        ///     This method is called after each mail is sent.
        /// </summary>
        /// <param name="mail">The mail that was sent.</param>
        protected virtual void OnMailSent(MailAttributes mail)
        {
        }

        /// <summary>
        ///     This method is called before each mail is sent
        /// </summary>
        /// <param name="context">
        ///     A simple context containing the mail
        ///     and a boolean value that can be toggled to prevent this
        ///     mail from being sent.
        /// </param>
        protected virtual void OnMailSending(MailSendingContext context)
        {
        }

        public virtual EmailResult Email(string viewName, object model = null, string masterName = null,
            bool trimBody = true)
        {
            if (viewName == null)
                throw new ArgumentNullException("viewName");

            var result = new EmailResult(this, MailSender, MailAttributes, viewName, masterName,
                MailAttributes.MessageEncoding, trimBody);
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

        private string FindAreaName()
        {
            string area = null;

            if (HttpContextBase != null &&
                HttpContextBase.Request != null &&
                HttpContextBase.Request.RequestContext != null &&
                HttpContextBase.Request.RequestContext.RouteData != null)
            {
                if (HttpContextBase.Request.RequestContext.RouteData.DataTokens.ContainsKey("area"))
                {
                    area = HttpContextBase.Request.RequestContext.RouteData.DataTokens["area"].ToString();
                }
            }

            if (area == null)
            {
                string name = GetType().Namespace;
                if (name != null && name.Contains(".Areas."))
                {
                    int startIndex = name.IndexOf(".Areas.", StringComparison.Ordinal) + 7;
                    int length = name.LastIndexOf(".", StringComparison.Ordinal) - startIndex;
                    area = name.Substring(startIndex, length);
                }
            }

            return area;
        }

        /// <summary>
        ///     Nothing to do here, left empty for now.
        /// </summary>
        protected override void ExecuteCore()
        {
        }

        /// <summary>
        ///     Dispose of the underlying MailSender when this controller is destroyed.
        /// </summary>
        /// <param name="disposing">Whether we are disposing or not.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (MailSender != null)
                {
                    MailSender.Dispose();
                    MailSender = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}