#region License

/* Copyright (C) 2012 by Scott W. Anderson
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
using ActionMailerNext.Implementations.SMTP;
using ActionMailerNext.Interfaces;
using FakeItEasy;
using NUnit.Framework;

namespace ActionMailerNext.Tests
{
    [TestFixture]
    public class DeliveryHelperTests
    {
        [Test]
        public void ConstructorWithNullInterceptorThrows()
        {
            var sender = A.Fake<IMailSender>();

            Assert.Throws<ArgumentNullException>(() => { new DeliveryHelper(sender, null); });
        }

        [Test]
        public void ConstructorWithNullSenderThrows()
        {
            var interceptor = A.Fake<IMailInterceptor>();

            Assert.Throws<ArgumentNullException>(() => { new DeliveryHelper(null, interceptor); });
        }

        [Test]
        public async void DeliverAsynchronouslyNotifiesWhenMailWasSent()
        {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailAttributes();

            await helper.DeliverAsync(mail);

            A.CallTo(() => interceptor.OnMailSent(A<MailAttributes>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void DeliverNotifiesWhenMailIsBeingSent()
        {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailAttributes();

            helper.Deliver(mail);

            A.CallTo(() => interceptor.OnMailSending(A<MailSendingContext>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void DeliverShouldAllowSendingToBeCancelled()
        {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            A.CallTo(() => interceptor.OnMailSending(A<MailSendingContext>.Ignored))
                .Invokes(x => x.GetArgument<MailSendingContext>(0).Cancel = true);

            helper.Deliver(new MailAttributes());

            A.CallTo(() => sender.Send(A<MailAttributes>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void DeliverSynchronouslyNotifiesWhenMailWasSent()
        {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailAttributes();

            helper.Deliver(mail);

            A.CallTo(() => interceptor.OnMailSent(mail)).MustHaveHappened();
        }

        [Test]
        public void DeliverSynchronouslySendsMessage()
        {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailAttributes();

            helper.Deliver(mail);

            A.CallTo(() => sender.Send(mail)).MustHaveHappened();
        }

        [Test]
        public void DeliverWithNullMailMessageThrows()
        {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);

            Assert.Throws<ArgumentNullException>(() => helper.Deliver(null));
        }

        [Test]
        public async void DeliveryAsynchronouslySendsMessage()
        {
            var sender = A.Fake<IMailSender>();
            var interceptor = A.Fake<IMailInterceptor>();
            var helper = new DeliveryHelper(sender, interceptor);
            var mail = new MailAttributes();

            await helper.DeliverAsync(mail);

            A.CallTo(() => sender.SendAsync(mail)).MustHaveHappened();
        }
    }
}