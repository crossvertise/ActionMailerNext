using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using RazorEngine;

namespace ActionMailer.Net.Standalone {
    /// <summary>
    /// An container for MailMessage with the appropriate body rendered by Razor.
    /// </summary>
    public class RazorEmailResult {
        private readonly IMailInterceptor _interceptor;
        private readonly DeliveryHelper _deliveryHelper;

        private readonly string _viewName;
        private readonly string _viewPath;

        /// <summary>
        /// The model object used to render the Razor template
        /// </summary>
        public readonly object Model;

        /// <summary>
        /// The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        public readonly MailMessage Mail;

        /// <summary>
        /// The IMailSender instance that is used to deliver mail.
        /// </summary>
        public readonly IMailSender MailSender;

        /// <summary>
        /// The default encoding used to send a message.
        /// </summary>
        public readonly Encoding MessageEncoding;

        /// <summary>
        /// Creates a new EmailResult.  You must call Compile() before this result
        /// can be successfully delivered.
        /// </summary>
        /// <param name="interceptor">The IMailInterceptor that we will call when delivering mail.</param>
        /// <param name="sender">The IMailSender that we will use to send mail.</param>
        /// <param name="mail">The mail message who's body needs populating.</param>
        /// <param name="viewName">The view to use when rendering the message body.</param>
        /// <param name="messageEncoding">The encoding to use when rendering a message.</param>
        /// <param name="viewPath">The path where we should search for the view.</param>
        /// <param name="model">The model that should be passed to the razor view during compilation.</param>
        public RazorEmailResult(IMailInterceptor interceptor, IMailSender sender, MailMessage mail, string viewName, Encoding messageEncoding, string viewPath, object model) {
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

            _interceptor = interceptor;
            MailSender = sender;
            Mail = mail;
            MessageEncoding = messageEncoding;
            Model = model;
            _viewName = viewName;
            _viewPath = viewPath;
            _deliveryHelper = new DeliveryHelper(sender, interceptor);
        }

        /// <summary>
        /// Sends your message.  This call will block while the message is being sent. (not recommended)
        /// </summary>
        public void Deliver() {
            _deliveryHelper.Deliver(false, Mail);
        }

        /// <summary>
        /// Sends your message asynchronously.  This method does not block.  If you need to know
        /// when the message has been sent, then override the OnMailSent method in MailerBase which
        /// will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public void DeliverAsync() {
            _deliveryHelper.Deliver(true, Mail);
        }

        /// <summary>
        /// Compiles the email body using the specified Razor view and model.
        /// </summary>
        public void Compile() {
            var textView = FindView("txt");
            var htmlView = FindView("html");

            if (htmlView == null && textView == null)
                throw new NoViewsFoundException(string.Format("Could not find any CSHTML or VBHTML views named [{0}] in the path [{1}].  Ensure that you specify the format in the file name (ie: {0}.txt.cshtml or {0}.html.cshtml)", _viewName, _viewPath));

            if (htmlView != null) {
                var body = Razor.Parse<dynamic>(htmlView, Model, _viewName);
                var altView = AlternateView.CreateAlternateViewFromString(body, MessageEncoding, MediaTypeNames.Text.Html);
                Mail.AlternateViews.Add(altView);
            }

            if (textView != null) {
                var body = Razor.Parse<dynamic>(textView, Model, _viewName);
                var altView = AlternateView.CreateAlternateViewFromString(body, MessageEncoding, MediaTypeNames.Text.Plain);
                Mail.AlternateViews.Add(altView);
            }
        }

        private string FindView(string extension) {
            var csViewFile = Path.Combine(_viewPath, string.Format("{0}.{1}.cshtml", _viewName, extension));
            var vbViewFile = Path.Combine(_viewPath, string.Format("{0}.{1}.vbhtml", _viewName, extension));

            if (File.Exists(csViewFile))
                return File.ReadAllText(csViewFile);

            if (File.Exists(vbViewFile))
                return File.ReadAllText(vbViewFile);

            return null;
        }
    }
}
