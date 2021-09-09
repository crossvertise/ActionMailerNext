using HandlebarsDotNet;

namespace ActionMailerNext.Standalone.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplateService : ITemplateService
    {
        private readonly IHandlebars _hbsService;
        private readonly ITemplateResolver _templateResolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateResolver"></param>
        /// <param name="viewSettings"></param>
        public TemplateService(ITemplateResolver templateResolver, ViewSettings viewSettings)
        {
            _templateResolver = templateResolver;
            _hbsService = Handlebars.Create();
            _hbsService.RegisterHelpers(viewSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="layout"></param>
        /// <returns></returns>
        public HandlebarsTemplate<object, object> Compile(string viewName, string layout = null)
        {
            if (!string.IsNullOrEmpty(layout))
            {
                AddTemplate(layout);
            }
            return _hbsService.Compile(_templateResolver.Resolve(viewName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        public void AddTemplate(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return;
            }

            _hbsService.RegisterTemplate(viewName, _templateResolver.Resolve(viewName));
        }
    }
}
