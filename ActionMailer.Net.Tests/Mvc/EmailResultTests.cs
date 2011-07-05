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
using ActionMailer.Net.Mvc;
using FakeItEasy;
using Xunit;

namespace ActionMailer.Net.Tests.Mvc {
    public class EmailResultTests {
        [Fact]
        public void ConstructorWithNullInterceptorThrows() {
            var mockSender = A.Fake<IMailSender>();

            Assert.Throws<ArgumentNullException>(() => {
                new EmailResult(null, mockSender, new MailMessage(), null, null, null);
            });
        }

        [Fact]
        public void ConstructorWithNullSenderThrows() {
            var mockInterceptor = A.Fake<IMailInterceptor>();

            Assert.Throws<ArgumentNullException>(() => {
                new EmailResult(mockInterceptor, null, new MailMessage(), null, null, null);
            });
        }

        [Fact]
        public void ConstructorWithNullMailMessageThrows() {
            var mockSender = A.Fake<IMailSender>();
            var mockInterceptor = A.Fake<IMailInterceptor>();

            Assert.Throws<ArgumentNullException>(() => {
                new EmailResult(mockInterceptor, mockSender, null, null, null, null);
            });
        }

        [Fact]
        public void DeliverShouldSendMailSynchronously() {
            var mockInterceptor = A.Fake<IMailInterceptor>();
            var mockSender = A.Fake<IMailSender>();
            var result = new EmailResult(mockInterceptor, mockSender, new MailMessage(), null, null, null);

            result.Deliver();

            A.CallTo(() => mockSender.Send(A<MailMessage>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void DeliverAsyncShouldSendMailAsynchronously() {
            var mockInterceptor = A.Fake<IMailInterceptor>();
            var mockSender = A.Fake<IMailSender>();
            var result = new EmailResult(mockInterceptor, mockSender, new MailMessage(), null, null, null);

            result.DeliverAsync();

            A.CallTo(() => mockSender.SendAsync(A<MailMessage>.Ignored, A<Action<MailMessage>>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void DeliverShouldCallOnMailSendingAndOnMailSent() {
            var mockInterceptor = A.Fake<IMailInterceptor>();
            var mockSender = A.Fake<IMailSender>();
            var result = new EmailResult(mockInterceptor, mockSender, new MailMessage(), null, null, null);

            result.Deliver();

            A.CallTo(() => mockInterceptor.OnMailSending(A<MailSendingContext>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            
            A.CallTo(() => mockInterceptor.OnMailSent(A<MailMessage>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void DeliverAsyncShouldCallOnMailSendingAndOnMailSent() {
            var mockInterceptor = A.Fake<IMailInterceptor>();
            var mockSender = A.Fake<IMailSender>();
            var result = new EmailResult(mockInterceptor, mockSender, new MailMessage(), null, null, null);

            // this ensures that when the email result calls SendAsync, the callback it
            // passes is actually fired, which is what we want to test here :)
            A.CallTo(() => mockSender.SendAsync(A<MailMessage>.Ignored, A<Action<MailMessage>>.Ignored))
                .Invokes(x => x.GetArgument<Action<MailMessage>>(1).Invoke(x.GetArgument<MailMessage>(0)));

            result.DeliverAsync();
            Thread.Sleep(100); // sleep slighty, just to make sure the callback is actually executed.

            A.CallTo(() => mockInterceptor.OnMailSending(A<MailSendingContext>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => mockInterceptor.OnMailSent(A<MailMessage>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void DeliverShouldNotSendIfOnMailSendingIsCancelled() {
            var mockInterceptor = A.Fake<IMailInterceptor>();
            var mockSender = A.Fake<IMailSender>();
            var mail = new MailMessage("no-reply@test.com", "test@test.com", "test subject", "body");
            var result = new EmailResult(mockInterceptor, mockSender, mail, null, null, null);

            // this ensures that the Cancel property on the MailSendingContext gets
            // set to "true", which should trigger the cancellation :)
            A.CallTo(() => mockInterceptor.OnMailSending(A<MailSendingContext>.Ignored))
                .Invokes(x => x.GetArgument<MailSendingContext>(0).Cancel = true);

            result.Deliver();

            A.CallTo(() => mockSender.Send(A<MailMessage>.Ignored)).MustNotHaveHappened();
        }
    }
}
