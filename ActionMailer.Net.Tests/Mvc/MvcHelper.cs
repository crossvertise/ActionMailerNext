using System;
using System.Collections;
using System.Web;
using FakeItEasy;

namespace ActionMailer.Net.Tests.Mvc {
    // Some helpers yanked from the MVC 3 source.
    public static class MvcHelper {
        public const string AppPathModifier = "/$(SESSION)";

        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod, string protocol, int port) {
            var httpContext = A.Fake<HttpContextBase>();

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

            A.CallTo(() => httpContext.Request.Url).Returns(uri);

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
            return GetHttpContext(appPath, requestPath, httpMethod, Uri.UriSchemeHttp, -1);
        }
    }
}
