#region License
/* Copyright (C) 2012 by Scott W. Anderson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System.Text;
using System.Web;
using System.Web.Mvc;
using ActionMailer.Net.Mvc;
using ActionMailer.Net.Tests.Mvc;

namespace ActionMailer.Net.Tests.Mvc {
    public class TestMailerBase : MailerBase {
        public TestMailerBase(IMailSender sender = null, Encoding defaultMessageEncoding = null)
            : base(sender, defaultMessageEncoding) { }
    }

    public class UTF8ViewEngine : IViewEngine {

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
            throw new System.NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
            return new ViewEngineResult(new UTF8View(), this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view) {
            throw new System.NotImplementedException();
        }
    }

    public class TextViewEngine : IViewEngine {
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
            throw new System.NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
            if (viewName.Contains("txt"))
                return new ViewEngineResult(new TextView(), this);

            return new ViewEngineResult(new[] { "" });
        }

        public void ReleaseView(ControllerContext controllerContext, IView view) {
            throw new System.NotImplementedException();
        }
    }

    public class MultipartViewEngine : IViewEngine {

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
            throw new System.NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
            if (viewName.Contains("txt"))
                return new ViewEngineResult(new TextView(), this);
            
            return new ViewEngineResult(new HtmlView(), this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view) {
            throw new System.NotImplementedException();
        }
    }

    public class WhiteSpaceViewEngine : IViewEngine {

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
            throw new System.NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
            return new ViewEngineResult(new WhiteSpaceView(), this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view) {
            throw new System.NotImplementedException();
        }
    }

    public class UTF8View : IView {
        public void Render(ViewContext viewContext, System.IO.TextWriter writer) {
            writer.Write("Umlauts are Über!");
        }
    }

    public class TextView : IView {
        public void Render(ViewContext viewContext, System.IO.TextWriter writer) {
            writer.Write("TextView");
        }
    }

    public class HtmlView : IView {
        public void Render(ViewContext viewContext, System.IO.TextWriter writer) {
            writer.Write("HtmlView");
        }
    }

    public class WhiteSpaceView : IView {
        public void Render(ViewContext viewContext, System.IO.TextWriter writer) {
            writer.Write("\r\n  \r\n This thing has leading and trailing whitespace.  \r\n \r\n");
        }
    }

    public class TestMailController : TestMailerBase {
        public TestMailController() {
            From = "test@test.com";
            Subject = "test subject";
        }

        public EmailResult TestMail() {
            return Email("TestView");
        }

        public EmailResult TestMaster() {
            return Email("TestView", masterName: "TestMaster");
        }
    }

    public class TestController : Controller {
        public string TestAction() {
            var email = new TestMailController { HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null) }.TestMail();
            return email.ViewName;
        }
    }
}

namespace ActionMailer.Net.Tests.Areas.TestArea.Controllers {
    public class MailController : TestMailerBase {
        public EmailResult TestEmail() {
            From = "test@test.com";
            Subject = "test subject";
            return Email("TestView");
        }
    }
}
