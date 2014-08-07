using System.IO;
using System.Web.Mvc;

namespace ActionMailer.Net.Mvc5_2.Tests.Mvc5_2
{
    public class UTF8View : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("Umlauts are Über!");
        }
    }
}