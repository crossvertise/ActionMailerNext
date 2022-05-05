using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

using ActionMailerNext.Interfaces;
using ActionMailerNext.Standalone.Helpers;
using ActionMailerNext.Standalone.Interfaces;
using ActionMailerNext.Utils;

namespace ActionMailerNext.Standalone
{
    /// <summary>
    /// An container for MailMessage with the appropriate body rendered by Razor.
    /// </summary>
    public class HBSEmailResult : IEmailResult
    {
        private MailAttributes _mailAttributes;
        private readonly Encoding _messageEncoding;
        private readonly ITemplateService _templateService;
        private readonly object _viewBag;

        private readonly string _viewName;
        private readonly string _viewPath;
        private readonly string _masterName;

        /// <summary>
        /// Creates a new EmailResult. You must call Compile() before this result can be successfully delivered.
        /// </summary>
        /// <param name="mailAttributes"> message who's body needs populating.</param>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="masterName">the main layout</param>
        /// <param name="viewPath">The path where we should search for the view.</param>
        /// <param name="templateService">The template service defining a ITemplateResolver and a TemplateBase</param>
        /// <param name="viewBag">The viewBag is a dynamic object that can transfer data to the view</param>
        /// <param name="messageEncoding"></param>
        public HBSEmailResult(MailAttributes mailAttributes, string viewName,
            Encoding messageEncoding, string masterName,
            string viewPath, ITemplateService templateService, dynamic viewBag)
        {
            _mailAttributes = mailAttributes ?? throw new ArgumentNullException("mailAttributes");
            _viewName = !string.IsNullOrWhiteSpace(viewName) ? viewName : throw new ArgumentNullException("viewName");
            _masterName = masterName;
            _viewPath = viewPath;

            _templateService = templateService ?? throw new ArgumentNullException("templateService");
            _messageEncoding = messageEncoding;

            _viewBag = viewBag;
        }

        /// <summary>
        /// The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        public MailAttributes MailAttributes
        {
            get { return _mailAttributes; }
            set { _mailAttributes = value; }
        }

        /// <summary>
        /// The default encoding used to send a messageBase.
        /// </summary>
        public Encoding MessageEncoding => _messageEncoding;

        /// <summary>
        /// Compiles the email body using the specified Razor view and model.
        /// </summary>
        public void Compile<T>(T model, bool trimBody, string externalViewPath = null)
        {
            string body = string.Empty;
            AlternateView altView;

            var hasTxtView = false;
            try
            {
                var itemplate = _templateService.Compile(_viewName, _masterName, externalViewPath);

                body = itemplate.Run(model, _viewBag);
            }
            catch (TemplateResolvingException)
            {
                if (!hasTxtView)
                    throw new NoViewsFoundException(
                        string.Format(
                            "Could not find any HBS views named [{0}] in the path [{1}].  Ensure that you specify the format in the file name (ie: {0}.hbs)",
                            _viewName, _viewPath));
            }
            catch (NullReferenceException ex)
            {
                var error = string.Format("{0}\n{1}\n{2}\n{3}\n\n{4}\nFile:{5}", ex.Message, ex.InnerException, ex.Data, ex.Source, ex.StackTrace, _viewName);
                throw new NullReferenceException(error);
            }


            if (trimBody)
                body = body.Trim();

            altView = AlternateView.CreateAlternateViewFromString(body, MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Html);
            MailAttributes.AlternateViews.Add(altView);
        }
    }
}