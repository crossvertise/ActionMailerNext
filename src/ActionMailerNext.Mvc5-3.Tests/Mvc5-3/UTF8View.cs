using System.IO;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5_3.Tests
{
    public class Utf8View : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("Umlauts are Über!");
        }
    }
}