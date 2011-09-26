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

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ActionMailer.Net.Mvc;

namespace ActionMailer.Net.Mvc {
    /// <summary>
    /// Simple extension methods for UrlHelper to always generate absolute urls.
    /// </summary>
    public static class UrlHelperExtensions {
        /// <summary>
        /// Generates an absolute url for the provided action.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteAction(this UrlHelper helper, string actionName) {
            var url = helper.Action(actionName);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided action.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteAction(this UrlHelper helper, string actionName, string controllerName) {
            var url = helper.Action(actionName, controllerName);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided action.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteAction(this UrlHelper helper, string actionName, object routeValues) {
            var url = helper.Action(actionName, routeValues);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided action.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteAction(this UrlHelper helper, string actionName, RouteValueDictionary routeValues) {
            var url = helper.Action(actionName, routeValues);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided action.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteAction(this UrlHelper helper, string actionName, string controllerName, object routeValues) {
            var url = helper.Action(actionName, controllerName, routeValues);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided action.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteAction(this UrlHelper helper, string actionName, string controllerName, RouteValueDictionary routeValues) {
            var url = helper.Action(actionName, controllerName, routeValues);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided action.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <param name="protocol">The protocol to use (http or https usually).</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteAction(this UrlHelper helper, string actionName, string controllerName, object routeValues, string protocol) {
            var url = helper.Action(actionName, controllerName, routeValues, protocol);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided action.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <param name="protocol">The protocol to use (http or https usually).</param>
        /// <param name="hostName">The host name to use.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteAction(this UrlHelper helper, string actionName, string controllerName, RouteValueDictionary routeValues, string protocol, string hostName) {
            var url = helper.Action(actionName, controllerName, routeValues, protocol, hostName);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided route.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <returns></returns>
        public static string AbsoluteRouteUrl(this UrlHelper helper, object routeValues) {
            var url = helper.RouteUrl(routeValues);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided route.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="routeName">The name of the route to use.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteRouteUrl(this UrlHelper helper, string routeName) {
            var url = helper.RouteUrl(routeName);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided route.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteRouteUrl(this UrlHelper helper, RouteValueDictionary routeValues) {
            var url = helper.RouteUrl(routeValues);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided route.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="routeName">The name of the route to use.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteRouteUrl(this UrlHelper helper, string routeName, object routeValues) {
            var url = helper.RouteUrl(routeName, routeValues);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided route.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="routeName">The name of the route to use.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteRouteUrl(this UrlHelper helper, string routeName, RouteValueDictionary routeValues) {
            var url = helper.RouteUrl(routeName, routeValues);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }
        
        /// <summary>
        /// Generates an absolute url for the provided route.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="routeName">The name of the route to use.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <param name="protocol">The protocol to use (http or https usually).</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteRouteUrl(this UrlHelper helper, string routeName, object routeValues, string protocol) {
            var url = helper.RouteUrl(routeName, routeValues, protocol);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        /// <summary>
        /// Generates an absolute url for the provided virtual content path.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="contentPath">The virtual path of the content.</param>
        /// <returns></returns>
        public static string AbsoluteContent(this UrlHelper helper, string contentPath) {
            var url = helper.Content(contentPath);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }
        
        /// <summary>
        /// Generates an absolute url for the provided route.
        /// </summary>
        /// <param name="helper">The UrlHelper instance to use for generation.</param>
        /// <param name="routeName">The name of the route to use.</param>
        /// <param name="routeValues">Any extra route values you wish to use for generation.</param>
        /// <param name="protocol">The protocol to use (http or https usually).</param>
        /// <param name="hostName">The host name to use.</param>
        /// <returns>An absolute url.</returns>
        public static string AbsoluteRouteUrl(this UrlHelper helper, string routeName, RouteValueDictionary routeValues, string protocol, string hostName) {
            var url = helper.RouteUrl(routeName, routeValues, protocol, hostName);
            return GetAbsoluteUrl(helper.RequestContext.HttpContext.Request, url).ToString();
        }

        private static Uri GetAbsoluteUrl(HttpRequestBase request, string url) {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
                return uri;

            url = String.Format("{0}{1}", request.Url.GetLeftPart(UriPartial.Authority), url);
            return new Uri(url);
        }
    }
}
