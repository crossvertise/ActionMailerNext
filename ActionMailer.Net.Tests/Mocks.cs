using System.Web;
using System.Web.Mvc;

namespace ActionMailer.Net.Tests {
    public class EmptyHttpContextBase : HttpContextBase { }

    public class TestViewEngine : IViewEngine {
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache) {
            throw new System.NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache) {
            return new ViewEngineResult(new TestView(), this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view) {
            throw new System.NotImplementedException();
        }
    }

    public class TestView : IView {
        public void Render(ViewContext viewContext, System.IO.TextWriter writer) {
            writer.Write("TestView");
        }
    }
}
