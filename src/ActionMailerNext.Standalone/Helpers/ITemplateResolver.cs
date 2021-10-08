using System.Collections.Generic;

namespace ActionMailerNext.Standalone.Helpers
{
    public interface ITemplateResolver
    {
        string Resolve(string name, string externalViewPath = null);

        List<MailTemplate> GetAllPartialTemplates();
    }
}