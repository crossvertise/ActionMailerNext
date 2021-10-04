using System.Collections.Generic;

namespace ActionMailerNext.Standalone.Helpers
{
    public interface ITemplateResolver
    {
        string Resolve(string name);

        List<MailTemplate> GetAllPartialTemplates();
    }
}