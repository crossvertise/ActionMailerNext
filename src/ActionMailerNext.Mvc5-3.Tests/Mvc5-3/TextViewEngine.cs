using System;
using System.Web.Mvc;

namespace ActionMailerNext.Mvc5_3.Tests
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
            return viewName.Contains("txt") ? new ViewEngineResult(new TextView(), this) : new ViewEngineResult(new[] {""});
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }
    }
}