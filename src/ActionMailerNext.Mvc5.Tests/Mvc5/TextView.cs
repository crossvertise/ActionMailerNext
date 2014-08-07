using System.IO;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5.Tests.Mvc5
{
    public class TextView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("TextView");
        }
    }
}