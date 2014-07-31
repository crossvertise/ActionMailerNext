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
using System.Collections;
using System.Web;
using FakeItEasy;
using System.Web.Routing;

namespace ActionMailer.Net.Tests.Mvc {
    // Some helpers yanked from the MVC 3 source.
    public static class MvcHelper {
        public const string AppPathModifier = "/$(SESSION)";

        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod, string protocol, int port, RouteData routeData) {
            var httpContext = A.Fake<HttpContextBase>();
            var requestContext = A.Fake<RequestContext>();

            // required because FakeItEasy will initialize this, and then MVC will use
            // it to determine if the Url was rewritten (which returns true, which
            // then causes an exception).
            A.CallTo(() => httpContext.Request.ServerVariables).Returns(null);

            if (!String.IsNullOrEmpty(appPath)) {
                A.CallTo(() => httpContext.Request.ApplicationPath).Returns(appPath);
            }
            if (!String.IsNullOrEmpty(requestPath)) {
                A.CallTo(() => httpContext.Request.AppRelativeCurrentExecutionFilePath).Returns(requestPath);
            }

            Uri uri;

            if (port >= 0)
                uri = new Uri(protocol + "://localhost" + ":" + Convert.ToString(port));
            else
                uri = new Uri(protocol + "://localhost");

            A.CallTo(() => requestContext.RouteData).Returns(routeData);
            
            A.CallTo(() => httpContext.Request.Url).Returns(uri);
            A.CallTo(() => httpContext.Request.RequestContext).Returns(requestContext);
            A.CallTo(() => httpContext.Request.PathInfo).Returns(string.Empty);

            if (!String.IsNullOrEmpty(httpMethod)) {
                A.CallTo(() => httpContext.Request.HttpMethod).Returns(httpMethod);
            }

            A.CallTo(() => httpContext.Session).Returns(null);
            A.CallTo(() => httpContext.Response.ApplyAppPathModifier((A<string>.Ignored)))
                .ReturnsLazily(x => AppPathModifier + x.Arguments[0]);

            A.CallTo(() => httpContext.Items).Returns(new Hashtable());
            return httpContext;
        }

        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod) {
            return GetHttpContext(appPath, requestPath, httpMethod, Uri.UriSchemeHttp, -1, null);
        }
    }
}
