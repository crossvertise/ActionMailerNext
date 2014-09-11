using System.Web.ModelBinding;
using RazorEngine.Templating;

namespace ActionMailerNext.Standalone.Helpers
{
    /// <summary>
    /// Extended Template base to add additional HTML Helpers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtendedTemplateBase<T> : TemplateBase<T>
    {
        private StandaloneHtmlHelpers<T> _html;
        private StandaloneUrlHelpers<T> _url;
        public StandaloneHtmlHelpers<T> Html
        {
            get
            {
                this._html = this._html ?? new StandaloneHtmlHelpers<T>(Model, TemplateService, ViewBag);
                return this._html;
            }
        }

        public StandaloneUrlHelpers<T> Url
        {
            get
            {
                this._url = this._url ?? new StandaloneUrlHelpers<T>(Model, TemplateService, ViewBag);
                return this._url;
            }
        }

     }

    public class ExtendedTemplateBase : TemplateBase
    {
        private StandaloneHtmlHelpers<object> _html;
        private StandaloneUrlHelpers<object> _url;
        public StandaloneHtmlHelpers<object> Html
        {
            get
            {
                this._html = this._html ?? new StandaloneHtmlHelpers<object>(TemplateService, ViewBag);
                return this._html;
            }
        }

        public StandaloneUrlHelpers<object> Url
        {
            get
            {
                this._url = this._url ?? new StandaloneUrlHelpers<object>(TemplateService, ViewBag);
                return this._url;
            }
        }

    }
}
