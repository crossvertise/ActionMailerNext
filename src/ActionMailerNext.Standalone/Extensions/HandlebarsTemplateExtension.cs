namespace ActionMailerNext.Standalone.Helpers
{
    using System.Collections.Generic;

    using HandlebarsDotNet;

    public static class HandlebarsTemplateExtension
    {
        public static string Run(this HandlebarsTemplate<object, object> self, object model, object viewbag)
        {
            var context = new
            {
                Model = model,
                ViewBag = viewbag,
                __Config = new Dictionary<string, object>()
            };

            return self(context);
        }
    }
}
