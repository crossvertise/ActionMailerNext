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

namespace ActionMailerNext.Mvc5_2.Tests
{
    // Some helpers yanked from the MVC 4 source.
    public static class MvcHelper
    {
        public const string AppPathModifier = "/$(SESSION)";

        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod,
            string protocol, int port)
        {
            var httpContext = A.Fake<HttpContextBase>();
            var request = A.Fake<HttpRequestBase>();
            var response = A.Fake<HttpResponseBase>();

            var httpWorkerRequest = A.Fake<HttpWorkerRequest>();

            Uri uri = port >= 0
                ? new Uri(protocol + "://localhost" + ":" + Convert.ToString(port))
                : new Uri(protocol + "://localhost");

            #region Request

            if (!String.IsNullOrEmpty(appPath))
            {
                A.CallTo(() => request.ApplicationPath).Returns(appPath);
                A.CallTo(() => request.RawUrl).Returns("/");
            }

            if (!String.IsNullOrEmpty(httpMethod))
            {
                A.CallTo(() => request.HttpMethod).Returns(httpMethod);
            }
            if (!String.IsNullOrEmpty(requestPath))
            {
                A.CallTo(() => request.AppRelativeCurrentExecutionFilePath).Returns(requestPath);
            }
            A.CallTo(() => request.Url).Returns(uri);
            A.CallTo(() => request.PathInfo).Returns(String.Empty);

            #endregion

            #region Response

            A.CallTo(() => response.ApplyAppPathModifier((A<string>.Ignored)))
                .ReturnsLazily(x => AppPathModifier + x.Arguments[0]);

            #endregion

            #region Context

            A.CallTo(() => httpContext.Request).Returns(request);
            A.CallTo(() => httpContext.Response).Returns(response);
            A.CallTo(() => httpContext.Session).Returns(null);
            A.CallTo(() => httpContext.GetService(typeof (HttpWorkerRequest))).Returns(httpWorkerRequest);
            A.CallTo(() => httpContext.Items).Returns(new Hashtable());

            #endregion

            return httpContext;
        }

        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod)
        {
            return GetHttpContext(appPath, requestPath, httpMethod, Uri.UriSchemeHttp, -1);
        }
    }
}