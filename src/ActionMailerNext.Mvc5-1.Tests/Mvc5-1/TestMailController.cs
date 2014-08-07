using System.Net.Mail;
using ActionMailerNext.Mvc5_1;

namespace ActionMailer.Net.Mvc5_1.Tests.Mvc5_1
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