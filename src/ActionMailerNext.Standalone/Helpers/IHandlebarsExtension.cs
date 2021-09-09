using HandlebarsDotNet;

namespace ActionMailerNext.Standalone.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class IHandlebarsExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hbsService"></param>
        /// <param name="viewBag"></param>
        public static void RegisterHelpers(this IHandlebars hbsService, ViewSettings viewSettings)
        {
            var helpers = new HandlebarsHelpers(hbsService, viewSettings);

            helpers.RegisterNewObjectHelper();
            helpers.RegisterActionLink();
            helpers.RegisterEmailButton();
            helpers.RegisterEmailLink();
        }
    }
}
