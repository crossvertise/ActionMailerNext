using System.Collections.Generic;
using RazorEngine.Templating;
using System;
using System.IO;

namespace ActionMailer.Net.Standalone {
    /// <summary>
    /// The RazorTemplateResolver tries to locate the templates using the standard search pattern of MVC
    /// and reads their content.
    /// </summary>
    public class RazorTemplateResolver : ITemplateResolver {
        private readonly string _viewPath;

        /// <summary>
        /// Creates a template resolver using the specified path.  If no path is given, this defaults to "Views"
        /// </summary>
        /// <param name="viewPath">The path containing your views</param>
        public RazorTemplateResolver(string viewPath) {
            _viewPath = viewPath ?? "Views";
        }

        /// <summary>
        /// Searches the view path for the given template and returns the contents of the view.
        /// Throws a TemplateResolvingException if no views could be found.
        /// </summary>
        /// <param name="name">The name of the view to search for</param>
        /// <returns>The contents of any views found.</returns>
        public string Resolve(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            var csViewName = name;
            var vbViewName = name;

            if (!csViewName.EndsWith(".cshtml"))
                csViewName += ".cshtml";

            if (!vbViewName.EndsWith(".vbhtml"))
                vbViewName += ".vbhtml";

            var appRoot = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            var csViewPath = Path.Combine(appRoot, _viewPath, csViewName);
            var vbViewPath = Path.Combine(appRoot, _viewPath, vbViewName);

            if (File.Exists(csViewPath))
                return File.ReadAllText(csViewPath);

            if (File.Exists(vbViewPath))
                return File.ReadAllText(vbViewPath);

            throw new TemplateResolvingException {SearchPaths = new List<string> {csViewPath, vbViewPath}};
        }
    }
}