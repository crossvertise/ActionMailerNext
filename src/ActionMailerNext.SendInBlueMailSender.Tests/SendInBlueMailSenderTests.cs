namespace ActionMailerNext.SendInBlue.Tests
{
    using ActionMailerNext.Interfaces;
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Net.Mime;

    [TestFixture]
    public class SendInBlueMailSenderTests
    {
        private readonly SendInBlueMailSender _sendInBlueMailSender;
        public SendInBlueMailSenderTests()
        {
            _sendInBlueMailSender = new SendInBlueMailSender("SendInBlue Api-Key", null);
        }

        [Test]
        public void Send_GivenCorrectMail_ReturnResponseWithQueuedStatus()
        {
            //Act
            var response = _sendInBlueMailSender.Send(
                 new MailAttributes()
                 {
                     To = new List<MailAddress>() { new MailAddress("Email Address") },
                     Subject = "SendInBlue Test",
                     Body = "test body",
                     TextBody = "Test sending email with SendInBlue from ActionMailer project",
                     AlternateViews = new List<AlternateView>() { new AlternateView("content.txt", MediaTypeNames.Text.Plain) },
                     From = new MailAddress("Email Address", "name"),
                     Tags = new List<string> { "Test" }
                 });

            //Assert
            Assert.AreEqual(response[0].DeliveryStatus, DeliveryStatus.QUEUED);
        }

        [Test]
        public void GenerateProspectiveMailMessage_GivenMailAttributesAreNotFilled_CorrespondFieldsShouldBeNull()
        {
            //Arrange
            var mailAttribute = new MailAttributes { From = new MailAddress("sample@crossvertise.com") };

            //Act
            var result = _sendInBlueMailSender.GenerateProspectiveMailMessage(mailAttribute);

            //Assert
            Assert.IsNull(result.Attachment);
            Assert.IsNull(result.Tags);
            Assert.IsNull(result.Bcc);
            Assert.IsNull(result.Cc);
            Assert.IsNull(result.Headers);

        }

        [Test]
        public void Deliver_GivenMailAttributesWithNullProps_ShouldThrowProperException()
        {
            //Arrange
            var mailAttribute = new MailAttributes();
            var mockEmailResult = new Mock<IEmailResult>();
            mockEmailResult.SetupGet(a => a.MailAttributes).Returns(mailAttribute);

            //Assert
            Assert.Throws<SendInBlueException>(() => _sendInBlueMailSender.Deliver(mockEmailResult.Object));

        }

        [Test]
        public void DeliverAsync_GivenMailAttributesWithNullProps_ShouldThrowProperException()
        {
            //Arrange
            var mailAttribute = new MailAttributes();
            var mockEmailResult = new Mock<IEmailResult>();
            mockEmailResult.SetupGet(a => a.MailAttributes).Returns(mailAttribute);

            //Assert
            Assert.ThrowsAsync<SendInBlueException>(async () => await _sendInBlueMailSender.DeliverAsync(mockEmailResult.Object));

        }
    }
}
