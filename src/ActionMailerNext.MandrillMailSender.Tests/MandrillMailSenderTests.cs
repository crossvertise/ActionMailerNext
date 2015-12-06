namespace ActionMailerNext.MandrillMailSender.Tests
{
    using System.Collections.Generic;
    using System.Net.Mail;

    using NUnit.Framework;

    [TestFixture]
    [Explicit]
    public class MandrillMailSenderTests
    {
        [Test]
        public void SendTest_SentToAddress()
        {
            var sender = new MandrillMailSender("enter your key", null);
            sender.Send(
                new MailAttributes()
                    {
                        To = new List<MailAddress>() { new MailAddress("test@test.com") },
                        Subject = "test",
                        Body = "test body",
                        From = new MailAddress("your email", "name")
                    });
        }
    }
}
