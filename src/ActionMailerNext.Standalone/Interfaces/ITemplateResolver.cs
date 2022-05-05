using System.Collections.Generic;
using ActionMailerNext.Standalone.Models;

namespace ActionMailerNext.Standalone.Interfaces
{
    public interface ITemplateResolver
    {
        string Resolve(string name, string externalViewPath = null);

        List<MailTemplate> GetAllPartialTemplates();
    }
}