namespace ActionMailerNext.PostProccesors
{
    using Interfaces;
    using PreMailer.Net;
    using System.IO;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;

    public class InlineCssPostProcessor : IPostProcessor
    {
        public MailAttributes Execute(MailAttributes mailAttributes)
        {
            var newMailAttributes = new MailAttributes(mailAttributes);

            foreach (var view in mailAttributes.AlternateViews)
            {
                using (var reader = new StreamReader(view.ContentStream))
                {
                    var body = reader.ReadToEnd();

                    if (view.ContentType.MediaType == MediaTypeNames.Text.Html)
                    {
                        var inlinedCssString = PreMailer.MoveCssInline(body);

                        byte[] byteArray = Encoding.UTF8.GetBytes(inlinedCssString.Html);
                        var stream = new MemoryStream(byteArray);
                        
                        var newAlternateView = new AlternateView(stream, MediaTypeNames.Text.Html);
                        newMailAttributes.AlternateViews.Add(newAlternateView);
                    }
                    else
                    {
                        newMailAttributes.AlternateViews.Add(view);
                    }
                }
            }

            return newMailAttributes;
        }
    }
}