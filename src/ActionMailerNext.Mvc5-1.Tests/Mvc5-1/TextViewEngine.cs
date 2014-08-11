using System;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5_1.Tests
{
    public class TextViewEngine : IViewEngine
    {
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName,
            bool useCache)
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName,
            bool useCache)
        {
            if (viewName.Contains("txt"))
                return new ViewEngineResult(new TextView(), this);

            return new ViewEngineResult(new[] {""});
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }
    }
}