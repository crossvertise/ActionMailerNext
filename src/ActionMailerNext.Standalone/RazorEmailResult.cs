using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;
using RazorEngine.Templating;

namespace ActionMailerNext.Standalone
{
    /// <summary>
    ///     An container for MailMessage with the appropriate body rendered by Razor.
    /// </summary>
    public class RazorEmailResult : IEmailResult
    {
        private readonly DeliveryHelper _deliveryHelper;
        private readonly IMailInterceptor _interceptor;
        private readonly IMailAttributes _mailAttributes;
        private readonly IMailSender _mailSender;
        private readonly Encoding _messageEncoding;
        private readonly ITemplateService _templateService;
        private readonly dynamic _viewBag;

        private readonly string _viewName;
        private readonly string _viewPath;
        private readonly string _masterName;

        /// <summary>
        ///     Creates a new EmailResult.  You must call Compile() before this result
        ///     can be successfully delivered.
        /// </summary>
        /// <param name="interceptor">The IMailInterceptor that we will call when delivering MailAttributes.</param>
        /// <param name="sender">The IMailSender that we will use to send MailAttributes.</param>
        /// <param name="mailAttributes"> message who's body needs populating.</param>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="masterName">the main layout</param>
        /// <param name="viewPath">The path where we should search for the view.</param>
        /// <param name="templateService">The template service defining a ITemplateResolver and a TemplateBase</param>
        /// <param name="viewBag">The viewBag is a dynamic object that can transfer data to the view</param>
        /// <param name="messageEncoding"></param>
        public RazorEmailResult(IMailInterceptor interceptor, IMailSender sender, IMailAttributes mailAttributes, string viewName,
            Encoding messageEncoding, string masterName,
            string viewPath, ITemplateService templateService, DynamicViewBag viewBag)
        {
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            if (sender == null)
                throw new ArgumentNullException("sender");

            if (mailAttributes == null)
                throw new ArgumentNullException("mailAttributes");

            if (string.IsNullOrWhiteSpace(viewName))
                throw new ArgumentNullException("viewName");


            if (templateService == null)
                throw new ArgumentNullException("templateService");

            _interceptor = interceptor;
            _mailSender = sender;
            _mailAttributes = mailAttributes;
            _viewName = viewName;
            _masterName = masterName;
            _viewPath = viewPath;
            _deliveryHelper = new DeliveryHelper(_mailSender, _interceptor);

            _templateService = templateService;
            _messageEncoding = messageEncoding;

            _viewBag = viewBag;
        }


        /// <summary>
        ///     The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        public IMailAttributes MailAttributes
        {
            get { return _mailAttributes; }
        }

        /// <summary>
        ///     The IMailSender instance that is used to deliver MailAttributes.
        /// </summary>
        public IMailSender MailSender
        {
            get { return _mailSender; }
        }

        /// <summary>
        ///     The default encoding used to send a messageBase.
        /// </summary>
        public Encoding MessageEncoding
        {
            get { return _messageEncoding; }
        }

        /// <summary>
        ///     Sends your message.  This call will block while the message is being sent. (not recommended)
        /// </summary>
        public void Deliver()
        {
            _deliveryHelper.Deliver(MailAttributes);
        }

        /// <summary>
        ///     Sends your message asynchronously.  This method does not block.  If you need to know
        ///     when the message has been sent, then override the OnMailSent method in MailerBase which
        ///     will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public async Task<IMailAttributes> DeliverAsync()
        {
            Task<IMailAttributes> deliverTask = _deliveryHelper.DeliverAsync(MailAttributes);
            return await deliverTask;
        }

        /// <summary>
        ///     Compiles the email body using the specified Razor view and model.
        /// </summary>
        public void Compile<T>(T model, bool trimBody)
        {
            string body;
            AlternateView altView;


            // Ensure master template is cached with _masterName and compiled with type object
            if (!String.IsNullOrWhiteSpace(_masterName))
            {
                _templateService.Resolve(_masterName, new object());
            }

            var itemplate = _templateService.Resolve(_viewName + ".html", model);
            var templateBase = itemplate as TemplateBase;
            if (templateBase != null && !String.IsNullOrWhiteSpace(_masterName))
            {
                templateBase.Layout = _masterName;
            }


            body = itemplate.Run(new ExecuteContext(_viewBag));


            if (trimBody)
                body = body.Trim();

            altView = AlternateView.CreateAlternateViewFromString(body, MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Html);
            MailAttributes.AlternateViews.Add(altView);
        }
    }
}