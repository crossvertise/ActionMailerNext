using System.IO;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5_1.Tests
{
    public class UTF8View : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("Umlauts are Über!");
        }
    }
}