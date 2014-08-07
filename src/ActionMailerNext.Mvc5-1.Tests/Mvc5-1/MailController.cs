using System.Net.Mail;
using ActionMailer.Net.Mvc5_1.Tests.Mvc5_1;
using ActionMailerNext.Mvc5_1;

namespace ActionMailer.Net.Mvc5_1.Tests.Areas.TestArea.Controllers
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