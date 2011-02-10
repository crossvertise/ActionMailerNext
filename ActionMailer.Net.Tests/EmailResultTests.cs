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
using Moq;
using Xunit;

namespace ActionMailer.Net.Tests {
    public class EmailResultTests {
        [Fact]
        public void ConstructorWithNullInterceptorThrows() {
            var mockSender = new Mock<IMailSender>();
        }

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
