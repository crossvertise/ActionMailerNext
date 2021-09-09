using System;
using System.Collections.Generic;
using System.IO;

namespace ActionMailerNext.Standalone.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class HandlebarsFilesTemplateResolver : ITemplateResolver
    {
        private readonly string _viewPath;

        /// <summary>
        ///     Creates a template resolver using the specified path.  If no path is given, this defaults to "Views"
        /// </summary>
        /// <param name="viewPath">The path containing your views</param>
        public HandlebarsFilesTemplateResolver(string viewPath)
        {
            _viewPath = viewPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Resolve(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            string csViewName = name;

            if (!csViewName.EndsWith(".hbs"))
                csViewName += ".hbs";

            var appRoot = AppDomain.CurrentDomain.BaseDirectory;

            var csViewPath = (csViewName).StartsWith("~")
                ? Path.GetFullPath(Path.Combine(appRoot, csViewName.Substring(2)))
                : Path.GetFullPath(Path.Combine(appRoot, _viewPath, csViewName));

            //Works with forward and backward slashes in the path
            if (File.Exists(csViewPath))
            {
                return File.ReadAllText(csViewPath);
            }
            throw new TemplateResolvingException { SearchPaths = new List<string> { csViewPath } };
        }
    }
}
