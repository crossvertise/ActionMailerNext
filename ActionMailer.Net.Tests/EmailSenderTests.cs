using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;
using System.Net.Mail;
using System.Threading;

namespace ActionMailer.Net.Tests {
    public class EmailSenderTests {
        [Fact]
        public void DeliverShouldSendMailSynchronously() {
            var mockInterceptor = new Mock<IMailInterceptor>();
            var mockSender = new Mock<IMailSender>();
            var result = new EmailResult(mockInterceptor.Object, mockSender.Object, new MailMessage(), null, null, null);

            result.Deliver();

            mockSender.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Exactly(1));
        }

        [Fact]
        public void DeliverAsyncShouldSendMailAsynchronously() {
            var mockInterceptor = new Mock<IMailInterceptor>();
            var mockSender = new Mock<IMailSender>();
            var result = new EmailResult(mockInterceptor.Object, mockSender.Object, new MailMessage(), null, null, null);

            result.DeliverAsync();

            mockSender.Verify(x => x.SendAsync(It.IsAny<MailMessage>(), It.IsAny<Action<MailMessage>>()), Times.Exactly(1));
        }

        [Fact]
        public void DeliverShouldCallOnMailSendingAndOnMailSent() {
            var mockInterceptor = new Mock<IMailInterceptor>();
            var mockSender = new Mock<IMailSender>();
            var result = new EmailResult(mockInterceptor.Object, mockSender.Object, new MailMessage(), null, null, null);

            result.Deliver();

            mockInterceptor.Verify(x => x.OnMailSending(It.IsAny<MailSendingContext>()), Times.Exactly(1));
            mockInterceptor.Verify(x => x.OnMailSent(It.IsAny<MailMessage>()), Times.Exactly(1));
        }

        [Fact]
        public void DeliverAsyncShouldCallOnMailSendingAndOnMailSent() {
            var mockInterceptor = new Mock<IMailInterceptor>();
            var mockSender = new Mock<IMailSender>();
            var result = new EmailResult(mockInterceptor.Object, mockSender.Object, new MailMessage(), null, null, null);

            // this ensures that when the email result calls SendAsync, the callback it
            // passes is actually fired, which is what we want to test here :)
            mockSender.Setup(x => x.SendAsync(It.IsAny<MailMessage>(), It.IsAny<Action<MailMessage>>()))
                .Callback((MailMessage msg, Action<MailMessage> func) => func(msg));
            result.DeliverAsync();
            Thread.Sleep(100); // sleep slighty, just to make sure the callback is actually executed.

            mockInterceptor.Verify(x => x.OnMailSending(It.IsAny<MailSendingContext>()), Times.Exactly(1));
            mockInterceptor.Verify(x => x.OnMailSent(It.IsAny<MailMessage>()), Times.Exactly(1));
        }

        [Fact]
        public void DeliverShouldNotSendIfOnMailSendingIsCancelled() {
            var mockInterceptor = new Mock<IMailInterceptor>();
            var mockSender = new Mock<IMailSender>();
            var mail = new MailMessage("no-reply@test.com", "test@test.com", "test subject", "body");
            var result = new EmailResult(mockInterceptor.Object, mockSender.Object, mail, null, null, null);

            // this ensures that the Cancel property on the MailSendingContext gets
            // set to "true", which should trigger the cancellation :)
            mockInterceptor.Setup(x => x.OnMailSending(It.IsAny<MailSendingContext>()))
                .Callback((MailSendingContext c) => c.Cancel = true);
            result.Deliver();

            mockSender.Verify(x => x.Send(It.IsAny<MailMessage>()), Times.Never());
        }
    }
}
