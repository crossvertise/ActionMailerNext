using System.Net.Mail;

namespace ActionMailerNext.Mvc5.Tests.Mvc5
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