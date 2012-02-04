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

using System;
using System.Web.Mvc;
using System.Web.Routing;
using ActionMailer.Net.Mvc;
using Xunit;

namespace ActionMailer.Net.Tests.Mvc {
    public class UrlHelperExtensionsTests {
        private readonly UrlHelper _helper;

        public UrlHelperExtensionsTests() {
            _helper = GetUrlHelper();
        }

        [Fact]
        public void AbsoluteActionWithAction() {
            var expectedUrl = String.Format("http://localhost{0}/app/home/test", MvcHelper.AppPathModifier);
            
            var generatedUrl = _helper.AbsoluteAction("test");

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteActionWithActionAndRouteData() {
            var expectedUrl = String.Format("http://localhost{0}/app/admin/test", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteAction("test", new {controller = "admin"});

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteActionWithActionAndRouteDictionary() {
            var values = new RouteValueDictionary(new {controller = "admin", id = "12345"});
            var expectedUrl = String.Format("http://localhost{0}/app/admin/test/12345", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteAction("test", values);

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteActionWithActionAndControllerName() {
            var expectedUrl = String.Format("http://localhost{0}/app/admin/test", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteAction("test", "admin");

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteActionWithActionAndControllerAndRouteValues() {
            var expectedUrl = String.Format("http://localhost{0}/app/admin/test/12345", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteAction("test", "admin", new {id = "12345"});

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteActionWithActionAndControllerAndRouteDictionary() {
            var expectedUrl = String.Format("http://localhost{0}/app/admin/test/12345", MvcHelper.AppPathModifier);
            var values = new RouteValueDictionary(new {id = "12345"});

            var generatedUrl = _helper.AbsoluteAction("test", "admin", values);

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteActionWithActionAndControllerAndRouteValuesAndProtocol() {
            var expectedUrl = String.Format("https://localhost{0}/app/admin/test/12345", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteAction("test", "admin", new { id = "12345" }, "https");

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteActionWithActionAndControllerAndRouteDictionaryAndProtocolAndHostname() {
            var expectedUrl = String.Format("https://google.com{0}/app/admin/test/12345", MvcHelper.AppPathModifier);
            var values = new RouteValueDictionary(new {id = "12345"});

            var generatedUrl = _helper.AbsoluteAction("test", "admin", values, "https", "google.com");
            

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteRouteUrlWithRouteName() {
            var expectedUrl = String.Format("http://localhost{0}/app/named/home/oldaction", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteRouteUrl("namedroute");

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteRouteUrlWithRouteValues() {
            var expectedUrl = String.Format("http://localhost{0}/app/home/oldaction/12345", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteRouteUrl(new {id = "12345"});

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteRouteUrlWithRouteDictionary() {
            var expectedUrl = String.Format("http://localhost{0}/app/home/oldaction/12345", MvcHelper.AppPathModifier);
            var values = new RouteValueDictionary(new {id = "12345"});

            var generatedUrl = _helper.AbsoluteRouteUrl(values);

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteRouteUrlWithRouteNameAndRouteValues() {
            var expectedUrl = String.Format("http://localhost{0}/app/named/home/oldaction/12345", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteRouteUrl("namedroute", new {id = "12345"});

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteRouteUrlWithRouteNameAndRouteDictionary() {
            var expectedUrl = String.Format("http://localhost{0}/app/named/home/oldaction/12345", MvcHelper.AppPathModifier);
            var values = new RouteValueDictionary(new { id = "12345" });

            var generatedUrl = _helper.AbsoluteRouteUrl("namedroute", values);

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteRouteUrlWithRouteNameAndRouteValuesAndProtocol() {
            var expectedUrl = String.Format("https://localhost{0}/app/named/home/oldaction/12345", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteRouteUrl("namedroute", new {id = "12345"}, "https");

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteRouteUrlWithRouteNameAndRouteDictionaryAndProtocolAndHostname() {
            var expectedUrl = String.Format("https://google.com{0}/app/named/home/oldaction/12345", MvcHelper.AppPathModifier);
            var values = new RouteValueDictionary(new { id = "12345" });

            var generatedUrl = _helper.AbsoluteRouteUrl("namedroute", values, "https", "google.com");

            Assert.Equal(expectedUrl, generatedUrl);
        }

        [Fact]
        public void AbsoluteContent() {
            var expectedUrl = String.Format("http://localhost{0}/app/content/abc.txt", MvcHelper.AppPathModifier);

            var generatedUrl = _helper.AbsoluteContent("~/content/abc.txt");

            Assert.Equal(expectedUrl, generatedUrl);
        }

        #region Helpers taken from MVC 3 source.
        private static RouteCollection GetRouteCollection() {
            var rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            return rt;
        }

        private static RouteData GetRouteData() {
            var rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");
            return rd;
        }

        private static UrlHelper GetUrlHelper() {
            return GetUrlHelper(GetRouteData(), GetRouteCollection());
        }

        private static UrlHelper GetUrlHelper(RouteData routeData = null, RouteCollection routeCollection = null) {
            var httpcontext = MvcHelper.GetHttpContext("/app/", null, null);
            var urlHelper = new UrlHelper(new RequestContext(httpcontext, routeData ?? new RouteData()), routeCollection ?? new RouteCollection());
            return urlHelper;
        }
        #endregion
    }
}
