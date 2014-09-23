using System.Net.Mail;
using ActionMailerNext.Mvc5.Tests.Mvc5;

namespace ActionMailerNext.Mvc5.Tests.Areas.TestArea.Controllers
{
    public class MailController : TestMailerBase
    {
        public EmailResult TestEmail()
        {
            MailAttributes.From = new MailAddress("test@test.com");
            MailAttributes.Subject = "test subject";
            return Email("TestView");
        }
    }
}