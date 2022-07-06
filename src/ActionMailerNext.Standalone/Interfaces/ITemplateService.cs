namespace ActionMailerNext.Standalone.Interfaces
{
    using HandlebarsDotNet;

    public interface ITemplateService
    {
        HandlebarsTemplate<object, object> Compile(string viewName, string layout = null, string externalViewPath = null);

        void AddTemplate(string viewName, string key = null, string externalViewPath = null);
    }
}
