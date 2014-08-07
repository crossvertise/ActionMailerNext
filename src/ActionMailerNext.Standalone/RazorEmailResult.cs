using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Utils;
using RazorEngine.Templating;

namespace ActionMailerNext.Standalone {
    /// <summary>
    /// An container for MailMessage with the appropriate body rendered by Razor.
    /// </summary>

    public class RazorEmailResult : IEmailResult {
        private readonly IMailInterceptor _interceptor;
        private readonly DeliveryHelper _deliveryHelper;
        private readonly ITemplateService _templateService;
        private readonly dynamic _viewBag;

        private readonly string _viewName;
        private readonly string _viewPath;

        
        /// <summary>
        /// The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        public IMailAttributes Mail { get { return _mail; } }
        private readonly IMailAttributes _mail;

        /// <summary>
        /// The IMailSender instance that is used to deliver mail.
        /// </summary>
        public  IMailSender MailSender {get { return _mailSender; } }
        private readonly IMailSender _mailSender;

        /// <summary>
        /// The default encoding used to send a messageBase.
        /// </summary>
        public Encoding MessageEncoding { get { return _messageEncoding; } }
        private readonly Encoding _messageEncoding;

        /// <summary>
        /// Creates a new EmailResult.  You must call Compile() before this result
        /// can be successfully delivered.
        /// </summary>
        /// <param name="interceptor">The IMailInterceptor that we will call when delivering mail.</param>
        /// <param name="sender">The IMailSender that we will use to send mail.</param>
        /// <param name="mail">The mail message who's body needs populating.</param>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="viewPath">The path where we should search for the view.</param>
        /// <param name="templateService">The template service defining a ITemplateResolver and a TemplateBase</param>
        /// <param name="viewBag">The viewBag is a dynamic object that can transfer data to the view</param>
        /// <param name="messageEncoding"></param>
        public RazorEmailResult(IMailInterceptor interceptor, IMailSender sender, IMailAttributes mail, string viewName,Encoding messageEncoding,
            string viewPath, ITemplateService templateService, DynamicViewBag viewBag) {
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            if (sender == null)
                throw new ArgumentNullException("sender");

            if (mail == null)
                throw new ArgumentNullException("mail");

            if (string.IsNullOrWhiteSpace(viewName))
                throw new ArgumentNullException("viewName");

            if (string.IsNullOrWhiteSpace(viewPath))
                throw new ArgumentNullException("viewPath");

            if (templateService == null)
                throw new ArgumentNullException("templateService");

            _interceptor = interceptor;
            _mailSender= sender;
            _mail = mail;
            _viewName = viewName;
            _viewPath = viewPath;
            _deliveryHelper = new DeliveryHelper(_mailSender, _interceptor);

            _templateService = templateService;
            _viewBag = viewBag;
            _messageEncoding = messageEncoding;
            }

        /// <summary>
        /// Sends your message.  This call will block while the message is being sent. (not recommended)
        /// </summary>
        public void Deliver() {
            _deliveryHelper.Deliver(Mail);
        }

        /// <summary>
        /// Sends your message asynchronously.  This method does not block.  If you need to know
        /// when the message has been sent, then override the OnMailSent method in MailerBase which
        /// will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public async Task<IMailAttributes> DeliverAsync()
        {
            var deliverTask = _deliveryHelper.DeliverAsync(Mail);
            return await deliverTask;
        }

        /// <summary>
        /// Compiles the email body using the specified Razor view and model.
        /// </summary>
        public void Compile<T>(T model, bool trimBody) {
            var hasTxtView = false;
            try {
                var body = _templateService.Resolve(_viewName + ".txt", model).Run(new ExecuteContext(_viewBag));
                if (trimBody)
                    body = body.Trim();

                var altView = AlternateView.CreateAlternateViewFromString(body,MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Plain);
                Mail.AlternateViews.Add(altView);
                hasTxtView = true;
            } catch (TemplateResolvingException) { }

            try {
                var body = _templateService.Resolve(_viewName + ".html", model).Run(new ExecuteContext(_viewBag));
                if (trimBody)
                    body = body.Trim();

                var altView = AlternateView.CreateAlternateViewFromString(body, MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Html);
                Mail.AlternateViews.Add(altView);
            } catch (TemplateResolvingException) {
                if (!hasTxtView)
                    throw new NoViewsFoundException(string.Format("Could not find any CSHTML or VBHTML views named [{0}] in the path [{1}].  Ensure that you specify the format in the file name (ie: {0}.txt.cshtml or {0}.html.cshtml)", _viewName, _viewPath));
            }
        }
    }
}
