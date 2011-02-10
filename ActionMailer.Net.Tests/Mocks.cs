#region License
/* Copyright (C) 2011 by Scott W. Anderson
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

using System.Web;
using System.Web.Mvc;

namespace ActionMailer.Net.Tests {
    public class EmptyHttpContextBase : HttpContextBase { }

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
}
