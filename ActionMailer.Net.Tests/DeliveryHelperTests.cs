#region License
/* Copyright (C) 2011 by Scott W. Anderson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

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
