using System.IO;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5.Tests.Mvc5
{
    public class WhiteSpaceView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("\r\n  \r\n This thing has leading and trailing whitespace.  \r\n \r\n");
        }
    }
}