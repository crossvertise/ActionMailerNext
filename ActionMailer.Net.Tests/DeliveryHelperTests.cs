using System;
using System.Net.Mail;
using System.Threading;
using FakeItEasy;
using Xunit;

namespace ActionMailer.Net.Tests {
    public class DeliveryHelperTests {
        [Fact]
        public void ConstructorWithNullSenderThrows() {
            var interceptor = A.Fake<IMailInterceptor>();

            Assert.Throws<ArgumentNullException>(() => {
                new DeliveryHelper(null, interceptor);
            });
        }

        [Fact]
        public void ConstructorWithNullInterceptorThrows() {
            var sender = A.Fake<IMailSender>();

            Assert.Throws<ArgumentNullException>(() => {
                new DeliveryHelper(sender, null);
            });
        }

        [Fact]
        public void DeliverWithNullMailMessageThrows() {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);

            Assert.Throws<ArgumentNullException>(() => helper.Deliver(false, null));
        }

        [Fact]
        public void DeliverSynchronouslySendsMessage() {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailMessage();

            helper.Deliver(false, mail);

            A.CallTo(() => sender.Send(mail)).MustHaveHappened();
        }

        [Fact]
        public void DeliveryAsynchronouslySendsMessage() {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailMessage();

            helper.Deliver(true, mail);

            A.CallTo(() => sender.SendAsync(mail, A<Action<MailMessage>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void DeliverNotifiesWhenMailIsBeingSent() {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);

            helper.Deliver(false, new MailMessage());

            A.CallTo(() => interceptor.OnMailSending(A<MailSendingContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void DeliverSynchronouslyNotifiesWhenMailWasSent() {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailMessage();

            helper.Deliver(false, mail);

            A.CallTo(() => interceptor.OnMailSent(mail)).MustHaveHappened();
        }

        [Fact]
        public void DeliverAsynchronouslyNotifiesWhenMailWasSent() {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailMessage();
            A.CallTo(() => sender.SendAsync(A<MailMessage>.Ignored, A<Action<MailMessage>>.Ignored))
                .Invokes(x => x.GetArgument<Action<MailMessage>>(1).Invoke(x.GetArgument<MailMessage>(0)));

            helper.Deliver(true, mail);
            Thread.Sleep(100); // give a small buffer for the callback to happen

            A.CallTo(() => interceptor.OnMailSent(mail)).MustHaveHappened();
        }

        [Fact]
        public void DeliverShouldAllowSendingToBeCancelled() {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            A.CallTo(() => interceptor.OnMailSending(A<MailSendingContext>.Ignored))
                .Invokes(x => x.GetArgument<MailSendingContext>(0).Cancel = true);

            helper.Deliver(false, new MailMessage());

            A.CallTo(() => sender.Send(A<MailMessage>.Ignored)).MustNotHaveHappened();
        }
    }
}
