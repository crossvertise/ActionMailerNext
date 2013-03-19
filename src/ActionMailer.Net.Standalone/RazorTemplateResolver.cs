using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using RazorEngine.Templating;

namespace ActionMailer.Net.Standalone
{
    /// <summary>
    /// The RazorTemplateResolver tries to locate the templates using the standard search pattern of MVC
    /// and reads their content.
    /// </summary>
    public class RazorTemplateResolver : ITemplateResolver
    {
        private string _viewPath;

        public RazorTemplateResolver(string viewPath)
        {
            _viewPath = viewPath ?? "Views";
        }

        public string Resolve(string name)
        {
            if (!name.EndsWith(".cshtml"))
                name += ".cshtml";

            var appRoot = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            var viewPath = Path.Combine(appRoot, _viewPath, name);

            if (File.Exists(viewPath))
                return File.ReadAllText(viewPath);
            else 
                throw new TemplateResolvingException();
        }
    }

    public class TemplateResolvingException : Exception
    {
        public List<string> SearchPaths { get; set; }


    }
}
