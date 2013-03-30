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

        public RazorTemplateResolver(string viewPath) {
            _viewPath = viewPath ?? "Views";
        }

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

            throw new TemplateResolvingException();
        }
    }
}