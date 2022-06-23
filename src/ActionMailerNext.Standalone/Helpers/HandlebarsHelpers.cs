namespace ActionMailerNext.Standalone.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Web;

    using HandlebarsDotNet;
    using HandlebarsDotNet.Compiler;
    using HandlebarsDotNet.PathStructure;

    using ActionMailerNext.Standalone.Models;

    public class HandlebarsHelpers
    {
        private readonly IHandlebars _hbsService;
        private readonly ViewSettings _viewSettings;
        private readonly string _resourcesDefaultNamespace;
        private readonly Func<string, string, HandlebarsException> missingParameterException =
            (helperName, argumentName) => new HandlebarsException(string.Format("{{{{0}}}} helper is missing an argument", helperName), new ArgumentNullException(argumentName));

        public HandlebarsHelpers(IHandlebars hbsService, ViewSettings viewSettings, string resourcesDefaultNamespace)
        {
            _hbsService = hbsService;
            _viewSettings = viewSettings;
            _resourcesDefaultNamespace = resourcesDefaultNamespace;
        }

        /// <summary>
        /// {{object key1=value1 key2='value2'}} or {{helperX (object key1=value1 key2='value2')}} 
        /// </summary>
        /// <return>
        /// Dictionary<string, object>
        /// </return>
        public void RegisterNewObject_Helper()
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
        /// {{tuple value1 value2}}
        /// </summary>
        /// <return>
        /// Tuple<object, object>
        /// </return>
        public void RegisterNewTuple_Helper()
        {
            _hbsService.RegisterHelper("tuple", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{tuple}} helper must have 2 argument");
                }

                var v1 = arguments[0];
                var v2 = arguments[1];

                return Tuple.Create(v1, v2);
            });
        }

        /// <summary>
        /// {{actionLink 'linkText' 'action' 'controller' (object key=value) (object key=value) 'protocol' 'host'}}
        /// </summary>
        /// <return>
        /// <a href=protocol://host/controller/action?routeValues>linkText</a>
        /// </return>
        public void RegisterActionLink_Helper()
        {
            _hbsService.RegisterHelper("actionLink", (output, context, arguments) =>
            {
                if (arguments.Length < 3 || arguments.Length > 7)
                {
                    throw new HandlebarsException("{{actionLink}} helper must have at least 3 arguments with maximum 7 arguments");
                }

                var linkText = arguments.At<string>(0);
                var actionName = arguments.At<string>(1);
                var controllerName = arguments.At<string>(2);
                var initialRouteValues = arguments.Length > 3 ? arguments.At<Dictionary<string, object>>(3) : null;
                var initialHtmlAttributes = arguments.Length > 4 ? arguments.At<Dictionary<string, object>>(4) : null;
                var protocol = arguments.Length > 5 ? arguments.At<string>(5) : null;
                var hostName = arguments.Length > 6 ? arguments.At<string>(6) : null;

                if (string.IsNullOrWhiteSpace(actionName))
                {
                    throw missingParameterException("actionLink", "actionName");
                }

                if (string.IsNullOrWhiteSpace(controllerName))
                {
                    throw missingParameterException("actionLink", "controllerName");
                }

                var routeValues = CastDictionary(initialRouteValues);
                if (initialRouteValues != null && routeValues == null)
                {
                    throw new ArgumentException("Couldn't Cast RouteValues. @{actionLink}");
                }

                var htmlAttributes = CastDictionary(initialHtmlAttributes);
                if (initialHtmlAttributes != null && htmlAttributes == null)
                {
                    throw new ArgumentException("Couldn't Cast HtmlAttributes. @{actionLink}");
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
        /// {{emailLink 'whatever@email.com'}}
        /// </summary>
        /// <return>
        /// <a></a>
        /// </return>
        public void RegisterEmailLink_Helper()
        {
            _hbsService.RegisterHelper("emailLink", (output, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{emailLink}} helper must have 1 argument");
                }

                var email = arguments[0].ToString();
                output.WriteSafeString($"<a href=\"mailto:{email}\">{email}</a>");
            });
        }

        /// <summary>
        /// {{emailButton 'linkText' (url 'action' 'controller' ...look at url helper) (object class='bold') (object key=value)}}
        /// tableAttributes, ahrefAttributes are optional
        /// </summary>
        /// <return>
        /// <a></a>
        /// </return>
        public void RegisterEmailButton_Helper()
        {
            _hbsService.RegisterHelper("emailButton", (output, context, arguments) =>
            {
                if (arguments.Length < 2 || arguments.Length > 4)
                {
                    throw new HandlebarsException("{{emailLink}} helper must have at least 2 arguments with maximum 4 arguments");
                }

                var linkText = arguments[0].ToString();
                var link = arguments[1].ToString();
                var initialTableAttributes = arguments.Length > 2 ? arguments[2] as Dictionary<string, object> : null;
                var initialahrefAttributes = arguments.Length > 3 ? arguments[3] as Dictionary<string, object> : null;

                var ahrefAttrStr = string.Empty;
                var tableAttrStr = string.Empty;

                var tableAttributes = CastDictionary(initialTableAttributes);
                var ahrefAttributes = CastDictionary(initialahrefAttributes);

                if (tableAttributes == null && initialTableAttributes != null)
                {
                    throw new ArgumentException("Couldn't Cast Table Attributes. @{emailButton}");
                }

                if (ahrefAttributes == null && initialahrefAttributes != null)
                {
                    throw new ArgumentException("Couldn't Cast ahref Attributes. @{emailButton}");
                }

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

        /// <summary>
        /// {{url 'action' 'controller' (object area='Manager') 'protocol' 'host'}}
        /// routeValues, protocol and hostName are optional
        /// Default host name, protocol and url pattern are fetched from ViewSettings
        /// </summary>
        /// <return>
        /// Url as string
        /// </return>
        public void RegisterUrl_Helper()
        {
            _hbsService.RegisterHelper("url", (context, arguments) =>
            {
                if (arguments.Length < 2 || arguments.Length > 5)
                {
                    throw new HandlebarsException("{{url}} helper must have at least 2 arguments with maximum 5 arguments");
                }

                var actionName = arguments[0].ToString();
                var controllerName = arguments[1].ToString();
                var initialRouteValues = arguments.Length > 2 ? arguments[2] as Dictionary<string, object> : null;
                var protocol = arguments.Length > 3 ? arguments[3].ToString() : null;
                var hostName = arguments.Length > 4 ? arguments[4].ToString() : null;

                if (string.IsNullOrWhiteSpace(actionName))
                {
                    throw missingParameterException("url", "actionName");
                }

                if (string.IsNullOrWhiteSpace(controllerName))
                {
                    throw missingParameterException("url", "controllerName");
                }

                var routeValues = CastDictionary(initialRouteValues);
                if (initialRouteValues != null && routeValues == null)
                {
                    throw new ArgumentException("Couldn't Cast RouteValues. @{url}");
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

                var link = uri.ToString();

                return link;
            });
        }

        /// <summary>
        /// {{stringFormat '{0} {1] {2]' 'Hello' 'World' '!'}}
        /// </summary>
        /// <return>
        /// Formatted string
        /// </return>
        public void RegisterStringFormat_Helper()
        {
            _hbsService.RegisterHelper("stringFormat", (output, context, arguments) =>
            {
                if (arguments.Length < 1)
                {
                    throw new HandlebarsException("{{stringFormat}} helper must have at least 1 argument");
                }

                var args = new string[arguments.Length - 1];
                for (var i = 0; i < args.Length; i++)
                {
                    args[i] = arguments[i + 1]?.ToString() ?? string.Empty;
                }

                output.WriteSafeString(string.Format(arguments[0].ToString(), args));
            });
        }

        /// <summary>
        /// {{resource 'ResourceFileName.Key'}}
        /// by default it searches for resource file in Xv.Infrastructure.Standard.Resources.(resourceFileName).(key) otherwise it should be used like
        /// {{resource 'Namespace.ResourceFileName.Key'}}
        /// 
        /// It searches for resources in 'de-de' culture file, if it didn't find it then it searches without specifying the culture(default resource file)
        /// </summary>
        /// <return>
        /// Resource's value as string
        /// </return>
        public void RegisterResourceResolver_Helper()
        {
            _hbsService.RegisterHelper("resource", (output, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{resource}} helper must have 1 argument");
                }

                var resource = arguments[0].ToString().Split('.');
                var resourceFile = string.Join(".", resource.Take(resource.Length - 1));
                var resourceKey = resource.Last();

                if (resource.Length == 2)
                {
                    resourceFile = $"{_resourcesDefaultNamespace}." + resourceFile;
                }

                // implement some cache
                var assembly = GetAssembly(resourceFile);

                var resourceManager = new ResourceManager(resourceFile, assembly);
                string val = string.Empty;

                try
                {
                    val = resourceManager.GetString(resourceKey, new System.Globalization.CultureInfo("de-de"));
                }
                catch (MissingManifestResourceException)
                {
                    val = resourceManager.GetString(resourceKey);
                }

                output.WriteSafeString(val);
            });
        }

        /// <summary>
        /// {{arrayToString Model.SomeListOrArray 'separator'}}
        /// Separator is optional and is by default ', '
        /// </summary>
        /// <return>
        /// string of array members separated by ', ' or the passed separator argument
        /// </return>
        public void RegisterArrayToString_Helper()
        {
            _hbsService.RegisterHelper("arrayToString", (output, context, arguments) =>
            {
                if (arguments.Length < 1 || arguments.Length > 2)
                {
                    throw new HandlebarsException("{{arrayToString}} helper must have at least 1 argument with maximum 2 arguments");
                }

                var array = arguments[0] as IEnumerable<string>;
                var separator = arguments.Length > 1 ? arguments[1].ToString() : ", ";

                var result = string.Join(separator, array);
                output.WriteSafeString(result);
            });
        }

        /// <summary>
        /// {{date '2021-01-01' 'd'}}
        /// {{date 'now' 'd'}}
        /// {{date 'nowUtc' 'd'}}
        /// {{date '2021-01-01' 'd' 'timeZoneId'}}
        /// {{date 'now' 'd' 'timeZoneId'}}
        /// {{date 'nowUtc' 'd' 'timeZoneId'}}
        /// </summary>
        /// <return>
        /// Formatted date as string
        /// </return>
        public void RegisterDate_Helper()
        {
            _hbsService.RegisterHelper("date", (HandlebarsHelper)((output, context, arguments) =>
            {
                if (arguments.Length != 2 && arguments.Length != 3)
                {
                    throw new HandlebarsException("{{date}} helper must have either 2 or 3 arguments");
                }

                var dateStr = arguments.At<string>(0);
                var format = arguments.At<string>(1);

                if (string.IsNullOrEmpty(dateStr))
                {
                    output.Write(string.Empty);
                    return;
                }

                if (!DateTime.TryParse(dateStr, out var date))
                {
                    if (string.Equals(dateStr, "now", StringComparison.InvariantCultureIgnoreCase))
                    {
                        date = DateTime.Now;
                    }
                    else if (string.Equals(dateStr, "nowUtc", StringComparison.InvariantCultureIgnoreCase))
                    {
                        date = DateTime.UtcNow;
                    }
                    else
                    {
                        throw new HandlebarsException("{{date}} helper couldn't convert first parameter to DateTime");
                    }
                }

                switch (arguments.Length)
                {
                    case 2:
                        output.WriteSafeString(date.ToString(format));
                        break;

                    case 3:
                        var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
                        try
                        {
                            var timeZone = arguments[2] is TimeZoneInfo info ? info : TimeZoneInfo.FindSystemTimeZoneById(arguments.At<string>(2));
                            output.WriteSafeString(TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZone).ToString(format));
                        }
                        catch (Exception ex)
                        {
                            throw new HandlebarsException("{{date}} couldn't get time zone id", ex);
                        }
                        break;
                }
            }));
        }

        /// <summary>
        /// {{number 100 'format'}}
        /// </summary>
        /// <return>
        /// Formatted number as string
        /// </return>
        public void RegisterNumber_Helper()
        {
            _hbsService.RegisterHelper("number", (output, context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{number}} helper must have 2 arguments");
                }

                if (double.TryParse(arguments[0].ToString(), out var number))
                {
                    output.WriteSafeString(number.ToString(arguments[1].ToString()));
                }
            });
        }

        /// <summary>
        /// {{multiply number1 number2}}
        /// </summary>
        /// <return>
        /// Number
        /// </return>
        public void RegisterMultiply_Helper()
        {
            _hbsService.RegisterHelper("multiply", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{multiply}} helper must have 2 arguments");
                }

                if (double.TryParse(arguments[0].ToString(), out var number1) && double.TryParse(arguments[1].ToString(), out var number2))
                {
                    return number1 * number2;
                }

                throw new HandlebarsException("Couldn't parse arguments to numbers");
            });
        }

        /// <summary>
        /// {{add number1 number2}}
        /// </summary>
        /// <return>
        /// Number
        /// </return>
        public void RegisterAdd_Helper()
        {
            _hbsService.RegisterHelper("add", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{add}} helper must have 2 arguments");
                }

                if (double.TryParse(arguments[0].ToString(), out var number1) && double.TryParse(arguments[1].ToString(), out var number2))
                {
                    return number1 + number2;
                }

                throw new HandlebarsException("Couldn't parse arguments to numbers");
            });
        }

        /// <summary>
        /// {{concat string1 string2 string3 ...}}
        /// </summary>
        /// <return>
        /// String
        /// </return>
        public void RegisterConcatString_Helper()
        {
            _hbsService.RegisterHelper("concat", (output, context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    throw new HandlebarsException("{{concat}} helper must have at least 2 arguments");
                }

                var str = new StringBuilder();
                foreach (var argument in arguments)
                {
                    str.Append(argument);
                }

                output.WriteSafeString(str);
            });
        }

        /// <summary>
        /// {{equal string string}}
        /// {{equal bool bool}}
        /// {{equal number number}}
        /// </summary>
        /// <return>
        /// Boolean
        /// </return>
        public void RegisterEqual_Helper()
        {
            _hbsService.RegisterHelper("equal", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{equal}} helper must have 2 arguments");
                }

                var arg1 = arguments[0];
                var arg2 = arguments[1];

                if (arg1 == null || arg2 == null)
                {
                    return false;
                }

                if (arg1 is string str1 && arg2 is string str2)
                {
                    return string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase);
                }

                if (arg1 is bool bool1 && arg2 is bool bool2)
                {
                    return bool1 == bool2;
                }

                if (double.TryParse(arg1.ToString(), out var dbl1) && double.TryParse(arg2.ToString(), out var dbl2))
                {
                    return dbl1 == dbl2;
                }

                return false;
            });
        }

        /// <summary>
        /// {{greater number number}}
        /// </summary>
        /// <return>
        /// Boolean
        /// </return>
        public void RegisterGreaterThan_Helper()
        {
            _hbsService.RegisterHelper("greater", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{greater}} helper must have 2 arguments");
                }

                var arg1 = arguments[0].ToString();
                var arg2 = arguments[1].ToString();

                if (double.TryParse(arg1, out var dbl1) && double.TryParse(arg2, out var dbl2))
                {
                    return dbl1 > dbl2;
                }

                if (DateTime.TryParse(arg1, out var dt1) && DateTime.TryParse(arg2, out var dt2))
                {
                    return dt1 > dt2;
                }

                return false;
            });
        }

        /// <summary>
        /// {{enumFlags 'Namespace.Enum' Flags}}
        /// Flags is a number or an object from the context with value like 'Gender.Male | Gender.Female' which will be sent also as number
        /// </summary>
        /// <return>
        /// List of enum members also as numbers, could be used within foreach then for example (enum resultMember) or (arrayToString result)
        /// </return>
        public void RegisterEnumFlags_Helper()
        {
            _hbsService.RegisterHelper("enumFlags", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{enumFlags}} helper must have 2 arguments");
                }

                var enumTypeName = arguments[0].ToString();
                if (!int.TryParse(arguments[1].ToString(), out var enumValue))
                {
                    return new List<string> { arguments[1].ToString() };
                }

                var enumType = GetEnumType(enumTypeName);

                var x = GetFlags(enumType, enumValue).Select(s => s.ToString()).ToList();
                return x;
            });
        }

        public void RegisterEnumHasFlag_Helper()
        {
            _hbsService.RegisterHelper("enumHasFlag", (context, arguments) =>
            {
                if (arguments.Length != 3)
                {
                    throw new HandlebarsException("{{enumHasFlag}} helper must have 2 arguments");
                }

                var enumTypeName = arguments[0].ToString();
                var enumType = GetEnumType(enumTypeName);


                int? searchFlag = null;
                if (arguments[2]?.GetType().FullName == enumTypeName)
                {
                    searchFlag = (int)arguments[2];
                }
                else if (int.TryParse(arguments[2].ToString(), out var searchEnumValue))
                {
                    searchFlag = searchEnumValue;
                }
                else
                {
                    foreach (var val in Enum.GetValues(enumType))
                    {
                        if (string.Equals(Enum.GetName(enumType, val), arguments[2].ToString(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            searchFlag = (int)val;
                            break;
                        }
                    }
                }

                if (searchFlag == null)
                {
                    return false;
                }

                if (!int.TryParse(arguments[1].ToString(), out var enumValue))
                {
                    return false;
                }

                return (enumValue & searchFlag) == searchFlag;
            });
        }

        /// <summary>
        /// {{enum 'Namespace.Enum' EnumValue}}
        /// </summary>
        /// <return>
        /// return the display Attribute of the enum member if found, otherwise it just returns its name
        /// </return>
        public void RegisterEnumDisplayName_Helper()
        {
            _hbsService.RegisterHelper("enum", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{enum}} helper must have 2 arguments");
                }

                var enumTypeName = arguments[0].ToString();
                var enumType = GetEnumType(enumTypeName);
                string enumMemberName = arguments[1].ToString();

                if (int.TryParse(arguments[1].ToString(), out var enumValue))
                {
                    enumMemberName = Enum.GetName(enumType, enumValue);
                }

                var enumMember = enumType.GetField(enumMemberName);

                if (enumMember != null &&
                    enumMember.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute attr)
                {
                    return attr.ResourceType != null ? attr.GetName() : attr.Name;
                }

                return enumMemberName;
            });
        }

        /// <summary>
        /// {{#elementAt array  index}}{{/elementAt}}
        /// index starts from 0
        /// to get elements from the end of the list 'minus' index should be specified (-1 is the last element, -2 is the second last, ...)
        /// It sets the scope of the block to the selected element.
        /// </summary>
        /// <return>
        /// Element at the specified index from the specified list or array
        /// </return>
        public void RegisterElementAt_BlockHelper()
        {
            _hbsService.RegisterHelper("elementAt", (output, options, context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{elementAt}} helper must have 2 arguments");
                }

                var list = arguments.At<IList>(0);
                var index = arguments.At<int>(1);
                var isInversed = index < 0;
                var absoluteIndex = Math.Abs(index);

                object obj = isInversed ? list[list.Count - absoluteIndex] : list[absoluteIndex];
                options.Template(output, obj);
            });
        }

        /// <summary>
        /// {{firstCharUpper 'hello world'}}
        /// </summary>
        /// <return>
        /// Same string with first character as capital (in the above example 'Hello world')
        /// </return>
        public void RegisterFirstCharToUpper_Helper()
        {
            _hbsService.RegisterHelper("firstCharUpper", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{firstCharUpper}} helper must have 1 argument");
                }

                var str = arguments.At<string>(0);

                if (string.IsNullOrEmpty(str))
                {
                    return string.Empty;
                }
                else if (str.Length == 1)
                {
                    return str.ToUpper();
                }
                else
                {
                    return char.ToUpper(str[0]) + str.Substring(1);
                }
            });
        }

        /// <summary>
        /// {{split string splitter}}
        /// {{split 'Hello, World' ','}} => ['Hello', 'World']
        /// </summary>
        /// <return>
        /// List of strings splitted by the specified splitter
        /// </return>
        public void RegisterSplit_Helper()
        {
            _hbsService.RegisterHelper("split", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{split}} helper must have 2 arguments");
                }

                var list = arguments.At<string>(0);
                var splitStr = arguments.At<string>(1);

                splitStr = splitStr.Replace("\\n", "\n");
                splitStr = splitStr.Replace("\\r", "\r");

                return list.Split(new string[] { splitStr }, StringSplitOptions.None).Select(s => s.Trim());
            });
        }

        /// <summary>
        /// {{#if.or bool1 bool2 bool3}}{{else}}{{/if.and}}
        /// Same like the standard if helper but it takes an infinite number of arguments and it will evaluate them with an 'Or' between each of them
        /// It also evaluates null objects, empty lists or strings as false
        /// It doesn't alter the scope.
        /// </summary>
        /// <return>
        /// Boolean
        /// </return>
        public void RegisterIfOr_BlockHelper()
        {
            _hbsService.RegisterHelper("if.or", (output, options, context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    throw new HandlebarsException("{{if.or}} helper must have at least 2 arguments");
                }

                if (arguments.Any(a => IsTrue(a)))
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse(output, context);
                }
            });
        }

        /// <summary>
        /// {{#if.and bool1 bool2 bool3}}{{else}}{{/if.and}}
        /// Same like the standard if helper but it takes an infinite number of arguments and it will evaluate them with an 'And' between each of them
        /// It also evaluates null objects, empty lists or strings as false
        /// It doesn't alter the scope.
        /// </summary>
        /// <return>
        /// Boolean
        /// </return>
        public void RegisterIfAnd_BlockHelper()
        {
            _hbsService.RegisterHelper("if.and", (output, options, context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    throw new HandlebarsException("{{if.and}} helper must have at least 2 arguments");
                }

                if (arguments.All(a => IsTrue(a)))
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse(output, context);
                }
            });
        }

        /// <summary>
        /// {{not bool}}
        /// Negates the passed value.
        /// Null objects, empty lists or strings are evaluated as false
        /// </summary>
        /// <return>
        /// Boolean
        /// </return>
        public void RegisterNot_Helper()
        {
            _hbsService.RegisterHelper("not", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{not}} helper must have 1 argument");
                }

                return !IsTrue(arguments[0]);
            });
        }

        /// <summary>
        /// {{set 'key' object @root}}
        /// It creates a key-value pair in the root context.__config (special dictionary included in the context)
        /// IMPORTANT last argument must be @root so that it can access the root context
        /// </summary>
        /// <return>
        /// Nothing
        /// </return>
        public void RegisterSetObj_Helper()
        {
            _hbsService.RegisterHelper("set", (Context context, Arguments arguments) =>
            {
                if (arguments.Length != 3)
                {
                    throw new HandlebarsException("{{set}} helper must have 3 arguments");
                }

                var key = arguments.At<string>(0);
                var value = arguments[1];
                var ctx = arguments.At<dynamic>(2);

                var config = ctx.__Config as Dictionary<string, object>;

                config[key] = value;

                return null;
            });
        }

        /// <summary>
        /// {{get 'key' @root}}
        /// It fetches the value of the specified key from the root context context.__config
        /// IMPORTANT last argument must be @root so that it can access the root context
        /// </summary>
        /// <return>
        /// the object with the specified key
        /// </return>
        public void RegisterGetObj_Helper()
        {
            _hbsService.RegisterHelper("get", (Context context, Arguments arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{get}} helper must have 2 arguments");
                }

                var key = arguments.At<string>(0);
                var ctx = arguments.At<dynamic>(1);

                var config = ctx.__Config as Dictionary<string, object>;

                if (config.TryGetValue(key, out var val))
                {
                    return val;
                }

                return null;
            });
        }

        /// <summary>
        /// {{#with object}}{{/with}}
        /// {{#with (get 'key')}}{{/with}}
        /// Sets the inside context to the specified object. Usually used with 'get' helper
        /// </summary>
        /// <return>
        /// Nothing
        /// </return>
        public void RegisterWith_BlockHelper()
        {
            _hbsService.RegisterHelper("with", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{with}} helper must have 1 argument");
                }

                var data = arguments[0];

                options.Template(output, data);
            });
        }

        /// <summary>
        /// {{enumAll 'Namespace.Enum'}}
        /// </summary>
        /// <return>
        /// Return list of all enum members inside the specified enum
        /// </return>
        public void RegisterEnumAll_Helper()
        {
            _hbsService.RegisterHelper("enumAll", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{enumAll}} helper must have 1 argument");
                }

                var enumType = GetEnumType(arguments.At<string>(0));
                return Enum.GetValues(enumType);
            });
        }

        /// <summary>
        /// {{#switch string @root}}{{#case 'somthing' @root}}{{/case}}{{#default @root}}{{/default}}{{/switch}}
        /// if not already in the root, then @root parameter must be sent at the end of each block
        /// Works like normal switch case (it works also with numbers)
        /// It doesn't alter the scope
        /// </summary>
        /// <return>
        /// Nothing
        /// </return>
        public void RegisterSwitchCase_BlockHelper()
        {
            _hbsService.RegisterHelper("switch", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1 && arguments.Length != 2)
                {
                    throw new HandlebarsException("{{switch}} helper must have 1 argument and the root context is optional argument");
                }

                var switchValue = arguments.At<string>(0);

                var root = arguments[1] as dynamic ?? options.Frame.Data.Value<dynamic>(ChainSegment.Root);
                var config = root.__Config as Dictionary<string, object>;
                config["switchValue"] = switchValue;
                config["caseValues"] = new List<string>();
                options.Template(output, context);
            });

            _hbsService.RegisterHelper("case", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1 && arguments.Length != 2)
                {
                    throw new HandlebarsException("{{case}} helper must have 1 argument and the root context is an optional argument");
                }

                var caseValue = arguments.At<string>(0);

                var root = arguments[1] as dynamic ?? options.Frame.Data.Value<dynamic>(ChainSegment.Root);
                var config = root.__Config as Dictionary<string, object>;
                var switchValue = config["switchValue"] as string;
                var caseValues = config["caseValues"] as List<string>;
                caseValues.Add(caseValue);
                if (string.Equals(switchValue, caseValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse();
                }
            });

            _hbsService.RegisterHelper("default", (output, options, context, arguments) =>
            {
                if (arguments.Length != 0 && arguments.Length != 1)
                {
                    throw new HandlebarsException("{{default}} helper can't have any arguments but the root context which is an optional argument");
                }

                var root = arguments[0] as dynamic ?? options.Frame.Data.Value<dynamic>(ChainSegment.Root);
                var config = root.__Config as Dictionary<string, object>;
                var switchValue = config["switchValue"] as string;
                var caseValues = config["caseValues"] as List<string>;
                if (caseValues.All(c => !string.Equals(c, switchValue, StringComparison.InvariantCultureIgnoreCase)))
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse();
                }
            });
        }

        /// <summary>
        /// {{emptyGuid}}
        /// </summary>
        /// <return>
        /// Return an empty Guid value as string
        /// </return>
        public void RegisterEmptyGuid_Helper()
        {
            _hbsService.RegisterHelper("emptyGuid", (output, context, arguments) =>
            {
                if (arguments.Length != 0)
                {
                    throw new HandlebarsException("{{emptyGuid}} helper can't have any arguments");
                }

                output.WriteSafeString(Guid.Empty.ToString("D"));
            });
        }

        /// <summary>
        /// {{orderBy list (tuple string_Property bool_isAscending) (tuple string_Property bool_isAscending) ...}}
        /// </summary>
        /// <return>
        /// Returns a list of order items
        /// </return>
        public void RegisterOrderBy_Helper()
        {
            _hbsService.RegisterHelper("orderBy", (context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    throw new HandlebarsException("{{orderBy}} helper must have at least 2 arguments");
                }

                var list = arguments.At<IList>(0).AsQueryable();
                var properties = arguments.Skip(1).Cast<Tuple<object, object>>().ToArray();

                if (!properties.All(p => p.Item1 is string && p.Item2 is bool))
                {
                    throw new ArgumentException("{{orderBy}} arguments are invalid", string.Join(" - ", properties.Select(p => p.Item1 + "=" + p.Item2)));
                }

                return GetOrderedList(list, properties.Select(p => (p.Item1.ToString(), (bool)p.Item2)).ToArray());
            });
        }

        /// <summary>
        /// {{count List}}
        /// </summary>
        /// <return>
        /// Writes the count of a list items
        /// </return>
        public void RegisterCount_Helper()
        {
            _hbsService.RegisterHelper("count", (output, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{count}} helper must have 1 argument");
                }

                if (arguments[0] is IList list)
                {
                    output.WriteSafeString(list.Count);
                }
            });
        }

        private bool IsTrue(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool bol)
            {
                return bol;
            }

            if (obj is string str && string.IsNullOrEmpty(str))
            {
                return false;
            }

            if (obj is IList list && list.Count == 0)
            {
                return false;
            }

            if (obj is Array arr && arr.Length == 0)
            {
                return false;
            }

            return true;
        }

        public List<object> GetOrderedList(IQueryable query, (string propertyName, bool isAscending)[] properties)
        {
            IQueryable<object> result = null;

            var type = query.GetType().GetGenericArguments().FirstOrDefault();

            for (var i = 0; i < properties.Length; i++)
            {
                var propertyName = properties[i].propertyName;
                var propertyDirection = properties[i].isAscending;

                string methodName = propertyDirection ? "OrderBy" : "OrderByDescending";
                IQueryable queryToOrder = query;

                if (i > 0)
                {
                    methodName = propertyDirection ? "ThenBy" : "ThenByDescending";
                    queryToOrder = result;
                }

                var parameter = Expression.Parameter(type, "p");

                var propertyType = type;

                Expression propertyAccess = parameter;
                foreach (var member in propertyName.Split('.'))
                {
                    propertyType = propertyType.GetProperty(member).PropertyType;
                    propertyAccess = Expression.PropertyOrField(propertyAccess, member);
                }

                var lambdaExpression = Expression.Lambda(propertyAccess, parameter);

                var orderExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { type, propertyType }, queryToOrder.Expression, Expression.Quote(lambdaExpression));
                result = queryToOrder.Provider.CreateQuery<object>(orderExpression);
            }

            return result.ToList();
        }

        private IEnumerable<Enum> GetFlags(Type enumType, int enumValue)
        {
            if (Convert.ToInt32(enumValue) == 0)
            {
                yield return (Enum)Enum.ToObject(enumType, 0);
                yield break;
            }

            var underlyingValue = Enum.ToObject(enumType, enumValue);

            foreach (Enum value in Enum.GetValues(enumType))
            {
                if (Convert.ToInt32(value) == 0)
                {
                    continue;
                }

                if (((Enum)underlyingValue).HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        private Type GetEnumType(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(name);
                if (type?.IsEnum ?? false)
                {
                    return type;
                }
            }

            foreach (var assembly in Directory.EnumerateFiles(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, "*.dll", SearchOption.AllDirectories).Select(file => Assembly.LoadFile(file)))
            {
                var type = assembly.GetType(name);
                if (type?.IsEnum ?? false)
                {
                    return type;
                }
            }

            return null;
        }

        private Assembly GetAssembly(string name)
        {
            var dllFiles = Directory.EnumerateFiles(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, "*.dll", SearchOption.AllDirectories);
            var assemblies = dllFiles.Select(f =>
            {
                try
                {
                    return Assembly.LoadFile(f);
                }
                catch
                {
                    return null;
                }
            }).Where(a => a != null).ToList();
            foreach (var assembly in assemblies)
            {
                if (assembly.GetManifestResourceNames().Where(n => n.EndsWith("resources")).Select(n => n.Replace(".resources", "")).Any(r => string.Equals(r, name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return assembly;
                }
            }

            return null;
        }

        private IDictionary<string, object> GetDictionary(HashParameterDictionary dictionary)
        {
            if (dictionary == null || !dictionary.Any())
            {
                return null;
            }

            var result = new Dictionary<string, object>();

            foreach (var kvp in dictionary)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            return result;

        }

        private Dictionary<string, string> CastDictionary(Dictionary<string, object> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }

            return dictionary.Select(d => new { d.Key, Value = d.Value?.ToString() })
                .ToDictionary(d => d.Key, d => d.Value);
        }
    }
}
