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
using System.IO;
using System.Text;
using FakeItEasy;
using Xunit;

namespace ActionMailer.Net.Tests.Standalone {
    public class RazorMailerBaseTests {
        [Fact]
        public void EmailWithNoViewNameShouldThrow() {
            var mockSender = A.Fake<IMailSender>();
            var mailer = new TestMailerBase(mockSender);

            Assert.Throws<ArgumentNullException>(() => mailer.Email(null));
        }

        [Fact]
        public void PassingAMailSenderShouldWork() {
            var mockSender = A.Fake<IMailSender>();

            var mailer = new TestMailerBase(mockSender);
            var email = mailer.Email("TextViewNoModel");

            Assert.Same(mockSender, mailer.MailSender);
            Assert.Same(mockSender, email.MailSender);
        }

        [Fact]
        public void PassingAnEncodingShouldWork() {
            var mockSender = A.Fake<IMailSender>();

            var mailer = new TestMailerBase(mockSender, Encoding.UTF8);
            var email = mailer.Email("UTF8TextView");
            var body = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd().Trim();

            Assert.Equal(Encoding.UTF8, email.MessageEncoding);
            Assert.Equal("Umlauts are Über!", body);
        }

        [Fact]
        public void RazorViewWithNoModelShouldRenderProperly() {
            var mockSender = A.Fake<IMailSender>();
            var mailer = new TestMailerBase(mockSender);

            var email = mailer.Email("TextViewNoModel");
            var body = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd().Trim();

            Assert.Equal("This is a test", body);
        }

        [Fact]
        public void PassingAModelShouldWork() {
            var mockSender = A.Fake<IMailSender>();
            var mailer = new TestMailerBase(mockSender);
            var model = new TestModel {
                Name = "Foo"
            };

            var email = mailer.Email("TextViewWithModel", model);
            var body = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd().Trim();

            Assert.Equal("Your name is:  Foo", body);
        }

        [Fact]
        public void MultipartMessagesShouldRenderBothViews() {
            var mockSender = A.Fake<IMailSender>();
            var mailer = new TestMailerBase(mockSender);

            var email = mailer.Email("MultipartNoModel");
            var textBody = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd().Trim();
            var htmlBody = new StreamReader(email.Mail.AlternateViews[1].ContentStream).ReadToEnd().Trim();

            Assert.Equal("Testing multipart.", textBody);
            Assert.Equal("<p>Testing multipart.</p>", htmlBody);
        }
    }
}
