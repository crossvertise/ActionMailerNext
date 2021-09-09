using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using HandlebarsDotNet;
using HandlebarsDotNet.Compiler;

namespace ActionMailerNext.Standalone.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class HandlebarsHelpers
    {
        private IHandlebars _hbsService;
        private ViewSettings _viewSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hbsService"></param>
        /// <param name="viewSettings"></param>
        public HandlebarsHelpers(IHandlebars hbsService, ViewSettings viewSettings)
        {
            _hbsService = hbsService;
            _viewSettings = viewSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterNewObjectHelper()
        {
            _hbsService.RegisterHelper("object", (context, arguments) =>
            {
                if (arguments.Length == 0)
                {
                    throw new HandlebarsException("{{object}} helper must have 1 argument");
                }

                return GetDictionary(arguments[0] as HashParameterDictionary);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterActionLink()
        {
            _hbsService.RegisterHelper("action", (output, context, arguments) =>
            {
                if (arguments.Length < 3)
                {
                    throw new HandlebarsException("{{action}} helper must have at least 3 arguments");
                }

                var linkText = arguments[0].ToString();
                var actionName = arguments[1].ToString();
                var controllerName = arguments[2].ToString();
                var routeValues = arguments.Length > 3 ? arguments[3] as Dictionary<string, string> : null;
                var htmlAttributes = arguments.Length > 4 ? arguments[4] as Dictionary<string, string> : null;
                var protocol = arguments.Length > 5 ? arguments[5].ToString() : null;
                var hostName = arguments.Length > 6 ? arguments[6].ToString() : null;

                if (string.IsNullOrWhiteSpace(actionName))
                {
                    throw new ArgumentNullException("actionName");
                }

                if (string.IsNullOrWhiteSpace(controllerName))
                {
                    throw new ArgumentNullException("controllerName");
                }

                Uri uri = null;
                if (routeValues != null && routeValues.Count != 0)
                {
                    routeValues.Add("action", actionName);
                    routeValues.Add("controller", controllerName);

                    uri = UtilHelper.BuildURI(protocol ?? _viewSettings.Protocol, hostName ?? _viewSettings.Hostname, _viewSettings.UrlPattern, routeValues);
                }
                else
                {
                    var uriBuilder = new UriBuilder(protocol ?? _viewSettings.Protocol, hostName ?? _viewSettings.Hostname)
                    {
                        Path = controllerName + "/" + actionName
                    };
                    uri = uriBuilder.Uri;
                }

                if (linkText == null)
                {
                    linkText = uri.ToString();
                }

                var htmlAttrStr = UtilHelper.ConvertDictionaryToString(htmlAttributes);
                var link = string.Format("<a {2}href=\"{0}\">{1}</a>", uri.ToString(), HttpUtility.HtmlEncode(linkText), htmlAttrStr);

                output.WriteSafeString(link);
            });
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void RegisterEmailLink()
        {
            _hbsService.RegisterHelper("emailLink", (output, context, arguments) =>
            {
                if (arguments.Length == 0)
                {
                    throw new HandlebarsException("{{emailLink}} helper must have 1 argument");
                }

                var email = arguments[0].ToString();
                output.WriteSafeString($"<a href=\"mailto:{email}\">{email}</a>");
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterEmailButton()
        {
            _hbsService.RegisterHelper("emailButton", (output, context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    throw new HandlebarsException("{{emailLink}} helper must have at least 2 arguments");
                }

                var linkText = arguments[0].ToString();
                var link = arguments[1].ToString();
                var tableAttributes = arguments.Length > 2 ? arguments[2] as Dictionary<string, string> : null;
                var ahrefAttributes = arguments.Length > 3 ? arguments[3] as Dictionary<string, string> : null;

                var ahrefAttrStr = string.Empty;
                var tableAttrStr = string.Empty;
                if (ahrefAttributes == null)
                {
                    ahrefAttributes = new Dictionary<string, string>
                {
                    {"style", "text-decoration:none"}
                };

                }
                ahrefAttrStr = UtilHelper.ConvertDictionaryToString(ahrefAttributes);

                if (tableAttributes == null)
                {
                    tableAttributes = new Dictionary<string, string>
                {
                    {"class", "button-dark"}
                };
                }
                tableAttrStr = UtilHelper.ConvertDictionaryToString(tableAttributes);

                var result = string.Format("<table {2}><tr><td><a {3} href=\"{0}\">{1}</a></td></tr></table>", link, linkText, tableAttrStr, ahrefAttrStr);
                output.WriteSafeString(result);
            });
        }

        private IDictionary<string, string> GetDictionary(HashParameterDictionary dictionary)
        {
            if (dictionary == null || !dictionary.Any())
            {
                return null;
            }

            var result = new Dictionary<string, string>();

            foreach (var kvp in dictionary)
            {
                result.Add(kvp.Key, kvp.Value.ToString());
            }

            return result;
        }
    }
}
