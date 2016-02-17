using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Utils;

namespace ActionMailerNext.Mvc5_3
{
    /// <summary>
    ///     A special result that should be returned from each action in your
    ///     MailAttributes controller.  Your controller must inherit from MailerBase.
    /// </summary>
    public class EmailResult : ViewResult, IEmailResult
    {
        private readonly IMailInterceptor _interceptor;
        private readonly bool _trimBody;

        private IView _htmlView;
        private string _htmlViewName;

        private IView _textView;
        private string _textViewName;

        /// <summary>
        ///     Creates a new EmailResult.  You must call ExecuteCore() before this result
        ///     can be successfully delivered.
        /// </summary>
        /// <param name="interceptor">The IMailInterceptor that we will call when delivering MailAttributes.</param>
        /// <param name="sender">The IMailSender that we will use to send MailAttributes.</param>
        /// <param name="mailAttributes">The MailAttributes messageBase who's body needs populating.</param>
        /// <param name="viewName">The view to use when rendering the messageBase body (can be null)</param>
        /// <param name="masterName">The maste rpage to use when rendering the messageBase body (can be null)</param>
        /// <param name="messageEncoding">The encoding to use when rendering a messageBase.</param>
        /// <param name="trimBody">Whether or not we should trim whitespace from the beginning and end of the messageBase body.</param>
        public EmailResult(IMailInterceptor interceptor, IMailSender sender, MailAttributes mailAttributes, string viewName,
            string masterName, Encoding messageEncoding, bool trimBody)
        {
            if (interceptor == null)
                throw new ArgumentNullException(nameof(interceptor));

            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            if (mailAttributes == null)
                throw new ArgumentNullException(nameof(mailAttributes));

            ViewName = viewName ?? ViewName;
            MasterName = masterName ?? MasterName;
            MessageEncoding = messageEncoding;
            MailAttributes = mailAttributes;
            MailSender = sender;
            _interceptor = interceptor;
            _trimBody = trimBody;
        }

        /// <summary>
        ///     The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        public MailAttributes MailAttributes { get; }

        /// <summary>
        ///     The IMailSender instance that is used to deliver MailAttributes.
        /// </summary>
        public IMailSender MailSender { get; }

        /// <summary>
        ///     The default encoding used to send a messageBase.
        /// </summary>
        public Encoding MessageEncoding { get; }

        /// <summary>
        ///     Sends your messageBase.  This call will block while the messageBase is being sent. (not recommended)
        /// </summary>
        public IList<IMailResponse> Deliver()
        {
            return MailSender.Send(MailAttributes);
        }

        /// <summary>
        ///     Sends your messageBase asynchronously.  This method does not block.  If you need to know
        ///     when the messageBase has been sent, then override the OnMailSent method in MailerBase which
        ///     will not fire until the asynchronous send operation is complete.
        /// </summary>
        public async Task<MailAttributes> DeliverAsync()
        {
            var deliverTask = MailSender.SendAsync(MailAttributes);
            await deliverTask.ContinueWith(t => AsyncSendCompleted(MailAttributes));

            return MailAttributes;
        }

        private void AsyncSendCompleted(MailAttributes mail)
        {
            _interceptor.OnMailSent(mail);
        }

        /// <summary>
        ///     Causes the body of the MailAttributes messageBase to be generated.
        /// </summary>
        /// <param name="context">The controller context to use while rendering the body.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            AddMessageViews(context);
        }

        private void LocateViews(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrEmpty(ViewName))
                ViewName = context.RouteData.GetRequiredString("action");

            _htmlViewName = $"{ViewName}.html";
            _textViewName = $"{ViewName}.txt";

            var htmlViewResult = ViewEngines.Engines.FindView(context, _htmlViewName, MasterName);
            if (htmlViewResult.View != null)
            {
                _htmlView = htmlViewResult.View;
            }

            var textViewResult = ViewEngines.Engines.FindView(context, _textViewName, MasterName);
            if (textViewResult.View != null)
            {
                _textView = textViewResult.View;
            }
        }

        private string RenderViewAsString(ControllerContext context, IView view)
        {
            using (var writer = new StringWriter())
            {
                var viewContext = new ViewContext(context, view, ViewData, TempData, writer);
                view.Render(viewContext, writer);

                var output = writer.GetStringBuilder().ToString();
                if (_trimBody)
                    output = output.Trim();

                return output;
            }
        }

        private void AddMessageViews(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            LocateViews(context);

            if (_textView == null && _htmlView == null)
            {
                string message =
                    $"You must provide a view for this email.  Views should be named {ViewName}.txt.cshtml or {ViewName}.html.cshtml (or aspx for WebFormsViewEngine) depending on the format you wish to render.";
                throw new NoViewsFoundException(message);
            }

            if (_textView != null)
            {
                var body = RenderViewAsString(context, _textView);
                var altView = AlternateView.CreateAlternateViewFromString(body,
                    MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Plain);
                MailAttributes.AlternateViews.Add(altView);
            }

            if (_htmlView != null)
            {
                var body = RenderViewAsString(context, _htmlView);
                var altView = AlternateView.CreateAlternateViewFromString(body,
                    MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Html);
                MailAttributes.AlternateViews.Add(altView);
            }
        }
    }
}