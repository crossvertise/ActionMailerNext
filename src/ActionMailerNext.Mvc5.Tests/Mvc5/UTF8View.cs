using System.IO;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5.Tests.Mvc5
{
    public class UTF8View : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("Umlauts are Über!");
        }
    }
}