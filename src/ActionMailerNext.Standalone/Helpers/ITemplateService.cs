using HandlebarsDotNet;

namespace ActionMailerNext.Standalone.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITemplateService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="layout"></param>
        /// <returns></returns>
        HandlebarsTemplate<object, object> Compile(string viewName, string layout = null, string externalViewPath = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        void AddTemplate(string viewName, string key = null, string externalViewPath = null);
    }
}
