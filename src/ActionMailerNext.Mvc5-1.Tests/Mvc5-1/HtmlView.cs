using System.IO;
using System.Web.Mvc;

namespace ActionMailer.Net.Mvc5_1.Tests.Mvc5_1
{
    public class HtmlView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("HtmlView");
        }
    }
}