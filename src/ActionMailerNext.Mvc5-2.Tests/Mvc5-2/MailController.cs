using System.Net.Mail;
using ActionMailer.Net.Mvc5_2.Tests.Mvc5_2;
using ActionMailerNext.Mvc5_2;

namespace ActionMailer.Net.Mvc5_2.Tests.Areas.TestArea.Controllers
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