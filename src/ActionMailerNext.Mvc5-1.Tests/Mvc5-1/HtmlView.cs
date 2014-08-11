using System.IO;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5_1.Tests
{
    public class HtmlView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("HtmlView");
        }
    }
}