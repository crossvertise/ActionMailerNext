namespace ActionMailerNext.Standalone.Implementations
{
    using System;
    using System.Linq;
    using System.Reflection;

    using HandlebarsDotNet;

    using ActionMailerNext.Standalone.Interfaces;
    using ActionMailerNext.Standalone.Helpers;
    using ActionMailerNext.Standalone.Models;

    public class TemplateService : ITemplateService
    {
        protected readonly IHandlebars _hbsService;
        private readonly ITemplateResolver _templateResolver;
        private readonly string _resourcesDefaultNamespace = "Xv.Infrastructure.Standard.Resources";
        private string _resourcesNamespace;

        public TemplateService(ITemplateResolver templateResolver, ViewSettings viewSettings)
        {
            _templateResolver = templateResolver;
            _hbsService = Handlebars.Create();
            RegisterHelpers(viewSettings);
            _templateResolver.GetAllPartialTemplates().ForEach(template =>
            {
                try
                {
                    _hbsService.RegisterTemplate(template.Key, template.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error at template key = {template.Key}", ex);
                }
            });
        }

        public string ResourcesNamespace
        {
            get
            {
                return string.IsNullOrEmpty(_resourcesNamespace) ? _resourcesDefaultNamespace : _resourcesNamespace;
            }
            set
            {
                _resourcesNamespace = value;
            }
        }

        public virtual void RegisterHelpers(ViewSettings viewSettings)
        {
            var helpers = new HandlebarsHelpers(_hbsService, viewSettings, ResourcesNamespace);

            var methods = helpers.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name.StartsWith("Register"));
            foreach (var method in methods)
            {
                method.Invoke(helpers, new object[0]);
            }
        }

        public HandlebarsTemplate<object, object> Compile(string viewName, string layout = null, string externalViewPath = null)
        {
            if (!string.IsNullOrEmpty(layout))
            {
                AddTemplate(layout, "_Layout");
            }

            try
            {
                return _hbsService.Compile(_templateResolver.Resolve(viewName, externalViewPath));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error at template = {viewName}", ex);
            }
        }

        public void AddTemplate(string viewName, string key = null, string externalViewPath = null)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return;
            }

            _hbsService.RegisterTemplate(string.IsNullOrEmpty(key) ? viewName : key, _templateResolver.Resolve(viewName, externalViewPath));
        }
    }
}
