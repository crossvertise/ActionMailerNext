using HandlebarsDotNet;

namespace ActionMailerNext.Standalone.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class HandlebarsTemplateExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="model"></param>
        /// <param name="viewbag"></param>
        /// <returns></returns>
        public static string Run(this HandlebarsTemplate<object, object> self, object model, object viewbag)
        {
            var context = new
            {
                Model = model,
                ViewBag = viewbag
            };
            return self(context);
        }
    }
}
