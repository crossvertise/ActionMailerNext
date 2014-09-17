using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ActionMailerNext.Standalone.Helpers
{
    public static class UtilHelper
    {
        public static Uri BuildURI(string protocol, string host, string formatString, IDictionary<string, string> routeDictionary)
        {
            var uriBuilder = new UriBuilder(protocol, host);
            var i = 0;
            var newFormatStringBuilder = new StringBuilder(formatString.ToLower());
            var queryDictionary = new Dictionary<string, string>();
            var keyToInt = new Dictionary<string, int>();
            foreach (var kvp in routeDictionary)
            {
                var key = kvp.Key.ToLower();
                var val = kvp.Value;
                if (formatString.IndexOf(key, StringComparison.Ordinal) < 0)
                {
                    queryDictionary.Add(key, val);
                }
                else
                {
                    newFormatStringBuilder = newFormatStringBuilder.Replace("{" + key + "}", "{" + i + "}");
                }
                keyToInt.Add(kvp.Key, i);
                i++;
            }
            uriBuilder.Query = FormatQuery(queryDictionary, false);

            
            var routeString = Regex.Replace(newFormatStringBuilder.ToString(), "{([A-Z])+}", "", RegexOptions.IgnoreCase);
            if (routeString.EndsWith("/"))
                routeString = routeString.Substring(0, routeString.Length - 1);
            uriBuilder.Path = String.Format(routeString, routeDictionary.OrderBy(x => keyToInt[x.Key]).Select(x => x.Value).ToArray()).Replace("//", "/");
           
            return uriBuilder.Uri;
        }

        public static string FormatQuery(IDictionary<string, string> queryDictionary, bool splitCommas = true)
        {
            var stringBuilder = new StringBuilder();
            foreach (var key in queryDictionary.Keys)
            {
                var entries = splitCommas ? queryDictionary[key].Split(',') : new[] { queryDictionary[key] };
                foreach (var value in entries)
                {
                    stringBuilder.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
                }
            }

            var queryString = stringBuilder.ToString();
            if (queryString.Length == 0)
            {
                return string.Empty;
            }

            return queryString.Substring(1);
        }

        public static string ConvertDictionaryToString(IDictionary<string, string> htmlAttributes)
        {
            return (htmlAttributes == null) ? string.Empty : htmlAttributes.Aggregate(string.Empty, (current, kvp) => current + (kvp.Key + " ='" + kvp.Value + "' "));
        }

        public static IDictionary<string,string> ObjectToDictionary(object value)
        {
            var dictionary = new Dictionary<string,string>();
            const BindingFlags bindingAttrs = BindingFlags.Public | BindingFlags.Instance;

            if (value != null)
            {
                foreach (
                    var property in
                        value.GetType().GetProperties(bindingAttrs).Where(property => property.CanRead))
                {
                    try
                    {
                        dictionary.Add(property.Name, property.GetValue(value).ToString());

                    }
                    catch (NullReferenceException)
                    {
                    }
                }
                return dictionary;
            }
            return new Dictionary<string, string>();
        }

        public static T GetAttribute<T>(this MemberInfo member, bool isRequired) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }

        public static string GetPropertyDisplayName<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayAttribute>(false);

            if (attr == null)
            {
                return memberInfo.Name;
            }
            if (attr.ResourceType == null)
            {
                return attr.Name;
            }

            var resourceManager = new ResourceManager(attr.ResourceType);
            return resourceManager.GetString(attr.Name);
        }

        public static string GetPropertyDisplayFormat<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                return "{0}";
            }

            var attr = memberInfo.GetAttribute<DisplayFormatAttribute>(false);

            return (attr == null || attr.DataFormatString == null) ? "{0}" : attr.DataFormatString;
        }
        public static string GetPropertyName<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            return memberInfo.Name;
        }
        
        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            Debug.Assert(propertyExpression != null, "propertyExpression != null");
            var memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                var unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            return memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property ? memberExpr.Member : null;
        }
    }
}
