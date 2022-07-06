namespace ActionMailerNext.Standalone.Interfaces
{
    using System.Collections.Generic;

    using ActionMailerNext.Standalone.Models;

    public interface ITemplateResolver
    {
        string Resolve(string name, string externalViewPath = null);

        List<MailTemplate> GetAllPartialTemplates();
    }
}