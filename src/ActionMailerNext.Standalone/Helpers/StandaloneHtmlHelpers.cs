using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace ActionMailerNext.Standalone.Helpers
{
    public class StandaloneHtmlHelpers<TModel>
    {
        private readonly ExecuteContext _executeContext;
        private readonly ITemplateService _templateService;
        private readonly ViewSettings _viewSettings;
        private readonly TModel _model;

        public StandaloneHtmlHelpers(ITemplateService templateService, dynamic viewBag, TModel model)
        {
            this._model = model;
            this._executeContext = (viewBag == null) ? new ExecuteContext() : new ExecuteContext(viewBag);
            this._templateService = templateService ?? new TemplateService();
            this._viewSettings = viewBag == null ? null : viewBag.ViewSettings as ViewSettings;

            //this._viewSettings = UtilHelper.GetDynamicMember(viewBag, "ViewSettings") as ViewSettings;
        }

        public IEncodedString Raw(string input)
        {
            return new RawString(input);
        }

        public TemplateWriter Partial(string templateName, object model = null)
        {
            
            var template = this._templateService.Resolve(templateName, model);
            if (template == null)
            {
                throw new ArgumentException("No template could be resolved with the name '" + templateName + "'");
            }
            var templateWriter = null as TemplateWriter;
            try
            {
                templateWriter = new TemplateWriter(tw => tw.Write(template.Run(this._executeContext)));
            }
            catch (TemplateCompilationException ex)
            {
                var error = string.Format("{0}\n{1}\n{2}\n{3}\n\n{4}\n(Partial){5}", ex.Message, ex.InnerException, ex.Data, ex.Source, ex.StackTrace, templateName);
                throw new Exception(error);
            }
            
            return templateWriter;
        }
        
        /// <summary>
        /// Returns an anchor element (a element) for the specified link text, action and controller.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>An anchor element (a element).</returns>
        public IEncodedString ActionLink(string linkText, string actionName, string controllerName)
        {
            return ActionLink(linkText, actionName, controllerName, null);
        }

        /// <summary>
        /// Returns an anchor element (a element) for the specified link text, action, controller, and route values.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An Object that contains the parameters for a route.</param>
        /// <returns>An anchor element (a element).</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEncodedString ActionLink(string linkText, string actionName, string controllerName, object routeValues)
        {
            var routerValuesDict = UtilHelper.ObjectToDictionary(routeValues);
            return ActionLink(linkText, actionName, controllerName, routerValuesDict, null);
        }

        /// <summary>
        /// Returns an anchor element (a element) for the specified link text, action, controller, and route values as a route value dictionary.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">Dictionary that contains the parameters for a route.</param>
        /// <returns>An anchor element (a element).</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEncodedString ActionLink(string linkText, string actionName, string controllerName, IDictionary<string, string> routeValues)
        {
            return ActionLink(linkText, actionName, controllerName, routeValues, null);
        }

        /// <summary>
        /// Returns an anchor element (a element) for the specified link text, action, controller, route values, and HTML attributes.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An Object that contains the parameters for a route.</param>
        /// <param name="htmlAttributes">An Object that contains the HTML attributes to set for the element.</param>
        /// <returns>An anchor element (a element).</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEncodedString ActionLink(string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, null, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element (a element) for the specified link text, action, controller, route values as a route value dictionary, and HTML attributes as a dictionary.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">Dictionary that contains the parameters for a route.</param>
        /// <param name="htmlAttributes">Dictionary that contains the HTML attributes to set for the element.</param>
        /// <returns>An anchor element (a element).</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEncodedString ActionLink(string linkText, string actionName, string controllerName, IDictionary<string, string> routeValues, IDictionary<string, string> htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, null, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element (a element) for the specified link text, action, controller, protocol, host name, route values, and HTML attributes.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="routeValues">An Object that contains the parameters for a route.</param>
        /// <param name="htmlAttributes">An Object that contains the HTML attributes to set for the element.</param>
        /// <returns>An anchor element (a element).</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEncodedString ActionLink(string linkText, string actionName, string controllerName, string protocol,
            string hostName, object routeValues, object htmlAttributes)
        {
            var routerValuesDict = UtilHelper.ObjectToDictionary(routeValues);
            var htmlAttributesDict = UtilHelper.ObjectToDictionary(htmlAttributes);
        
            return ActionLink(linkText, actionName, controllerName, protocol, hostName, routerValuesDict, htmlAttributesDict);
        }

        /// <summary>
        /// Returns an anchor element (a element) for the specified link text, action, controller, protocol, host name, route values as a route value dictionary, and HTML attributes as a dictionary.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="routeValues">Dictionary that contains the parameters for a route.</param>
        /// <param name="htmlAttributes">Dictionary that contains the HTML attributes to set for the element.</param>
        /// <returns>An anchor element (a element).</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEncodedString ActionLink(string linkText, string actionName, string controllerName, string protocol, string hostName, IDictionary<string, string> routeValues, IDictionary<string, string> htmlAttributes)
        {
            if (string.IsNullOrWhiteSpace(actionName))
            {
                throw new ArgumentNullException("actionName");
            }

            if (string.IsNullOrWhiteSpace(controllerName))
            {
                throw new ArgumentNullException("controllerName");
            }

            //var uriBuilder = new UriBuilder(protocol ?? _viewSettings.Protocol, hostName?? _viewSettings.Hostname);
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

            return new RawString(link);
        }
        
        public IEncodedString EmailLinkGenerator(string email)
        {
            var emailLink = string.Format("<a href=\"mailto:{0}\">{0}</a>", email);
            return new RawString(emailLink);
        }

        public IEncodedString EmailButton(string linkText, string link, IDictionary<string, string> tableAttributes = null, IDictionary<string, string> ahrefAttributes = null)
        {
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
            return new RawString(result);
        }

        public IEncodedString EmailButton(string linkText, IEncodedString link, IDictionary<string, string> tableAttributes = null, IDictionary<string, string> ahrefAttributes = null)
        {
            return EmailButton(linkText, link.ToEncodedString(), tableAttributes, ahrefAttributes);
        }


        public IEncodedString LabelFor<TValue>(Expression<Func<TModel, TValue>> expression, object htmlAttributes = null, string labelText = null)
        {
            var resolvedLabelText = labelText ?? UtilHelper.GetPropertyDisplayName(expression);
            var propName = UtilHelper.GetPropertyName(expression);
            var htmlAttributesStr = string.Empty;

            if (String.IsNullOrEmpty(resolvedLabelText))
            {
                return new HtmlEncodedString(string.Empty);
            }
            if (htmlAttributes != null)
            {
                var htmlAttributesDict = UtilHelper.ObjectToDictionary(htmlAttributes);
                htmlAttributesStr = UtilHelper.ConvertDictionaryToString(htmlAttributesDict);
            }
            var tag = string.Format("<label {2}for=\"{0}\">{1}</label>", propName, HttpUtility.HtmlEncode(resolvedLabelText), htmlAttributesStr);
            return new RawString(tag);
        }

        public IEncodedString DisplayNameFor<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var resolvedLabelText = UtilHelper.GetPropertyDisplayName(expression);

            if (String.IsNullOrEmpty(resolvedLabelText))
            {
                return new HtmlEncodedString(string.Empty);
            }

            return new HtmlEncodedString(resolvedLabelText);
        }
        
        public IEncodedString DisplayFor<TValue>(Expression<Func<TModel,TValue>> expression)
        {
            
            var value = string.Empty;
            try
            {
                var result = expression.Compile()(_model);
                value = string.Format(UtilHelper.GetPropertyDisplayFormat(expression), result);
            }
            catch (NullReferenceException ex)
            {
                return new RawString(string.Empty);
            }
            return new RawString(value);
        }

        public IEncodedString NewLineToBr(string input)
        {
            if (input == null)
            {
                return new HtmlEncodedString(string.Empty);
            }

            var encodedString = new HtmlEncodedString(input);
            return new RawString(NewLineToBrString(encodedString.ToEncodedString()));
        }

        private string NewLineToBrString(string input)
        {
            return input.Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
        }

    }
}
