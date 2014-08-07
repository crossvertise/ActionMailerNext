using System.Net.Mail;
using ActionMailerNext.Mvc5_2.Tests.Mvc5_2;

namespace ActionMailerNext.Mvc5_2.Tests.Areas.TestArea.Controllers
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