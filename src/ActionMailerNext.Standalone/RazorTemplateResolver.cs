using System;
using System.Collections.Generic;
using System.IO;
using RazorEngine.Templating;

namespace ActionMailerNext.Standalone
{
    /// <summary>
    ///     The RazorTemplateResolver tries to locate the templates using the standard search pattern of MVC
    ///     and reads their content.
    /// </summary>
    public class RazorTemplateResolver : ITemplateResolver
    {
        private readonly string _viewPath;

        /// <summary>
        ///     Creates a template resolver using the specified path.  If no path is given, this defaults to "Views"
        /// </summary>
        /// <param name="viewPath">The path containing your views</param>
        public RazorTemplateResolver(string viewPath)
        {
            _viewPath = viewPath ?? "Views";
        }

        /// <summary>
        ///     Searches the view path for the given template and returns the contents of the view.
        ///     Throws a TemplateResolvingException if no views could be found.
        /// </summary>
        /// <param name="name">The name of the view to search for</param>
        /// <returns>The contents of any views found.</returns>
        public string Resolve(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            string csViewName = name;
            string vbViewName = name;

            if (!csViewName.EndsWith(".cshtml"))
                csViewName += ".cshtml";

            if (!vbViewName.EndsWith(".vbhtml"))
                vbViewName += ".vbhtml";

            string appRoot = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            //Works with forward and backward slashes in the path
            string csViewPath = Path.GetFullPath(Path.Combine(appRoot, _viewPath, csViewName));
            string vbViewPath = Path.GetFullPath(Path.Combine(appRoot, _viewPath, vbViewName));

            if (File.Exists(csViewPath))
                return File.ReadAllText(csViewPath);

            if (File.Exists(vbViewPath))
                return File.ReadAllText(vbViewPath);

            throw new TemplateResolvingException {SearchPaths = new List<string> {csViewPath, vbViewPath}};
        }
    }
}