using System.IO;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5_2.Tests
{
    public class WhiteSpaceView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write("\r\n  \r\n This thing has leading and trailing whitespace.  \r\n \r\n");
        }
    }
}