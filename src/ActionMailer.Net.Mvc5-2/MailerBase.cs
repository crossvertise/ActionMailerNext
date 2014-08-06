
namespace ActionMailer.Net.Mvc5_2 {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Interfaces;


    /// <summary>
    /// The base class that your controller should inherit from if you wish
    /// to send emails through ActionMailer.Net.
    /// </summary>
    public abstract class MailerBase : Controller, IMailInterceptor
    {

        public IMailAttributes MailAttributes;
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
        protected virtual void OnMailSent(IMailAttributes mail) { }

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

        void IMailInterceptor.OnMailSent(IMailAttributes mail) {
            OnMailSent(mail);
        }

        public void SetMailMethod(MailMethod method)
        {
            MailAttributes = MailMethodUtil.GetAttributes(method);
            MailSender = MailMethodUtil.GetSender(method);
        }

        /// <summary>
        /// Initializes MailerBase using the defaultMailSender and system Encoding.
        /// </summary>
        /// <param name="mailAttributes"> the mail attributes</param>
        /// <param name="mailSender">The underlying mail sender to use for delivering mail.</param>
        protected MailerBase(IMailAttributes mailAttributes = null , IMailSender mailSender = null)
        {
            MailAttributes = mailAttributes ?? MailMethodUtil.GetAttributes();
            MailSender = mailSender ?? MailMethodUtil.GetSender();

            if (System.Web.HttpContext.Current == null) return;
            HttpContextBase = new HttpContextWrapper(System.Web.HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(HttpContextBase) ?? new RouteData();
            var requestContext = new RequestContext(HttpContextBase, routeData);
            base.Initialize(requestContext);
        }

        public virtual EmailResult Email(string viewName, object model = null, string masterName = null, bool trimBody = true) {
            if (viewName == null)
                throw new ArgumentNullException("viewName");

            var result = new EmailResult(this, MailSender, MailAttributes, viewName, masterName, MailAttributes.MessageEncoding, trimBody);
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