using System.Net.Mail;
using ActionMailerNext.Mvc5_2;
using ActionMailerNext.Mvc5_2.Tests.Mvc5_2;

namespace ActionMailer.Net.Mvc5_2.Tests.Mvc5_2
{
    public class TestMailController : TestMailerBase
    {
        public TestMailController()
        {
            MailAttributes.From = new MailAddress("test@test.com");
            MailAttributes.Subject = "test subject";
        }

        public EmailResult TestMail()
        {
            return Email("TestView");
        }

        public EmailResult TestMaster()
        {
            return Email("TestView", masterName: "TestMaster");
        }
    }
}