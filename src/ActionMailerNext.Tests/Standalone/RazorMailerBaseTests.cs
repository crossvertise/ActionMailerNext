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
using ActionMailerNext.Implementations.SMTP;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Standalone;
using FakeItEasy;
using NUnit.Framework;

namespace ActionMailerNext.Tests.Standalone
{
    [TestFixture]
    public class RazorMailerBaseTests
    {
        [Test]
        public void EmailWithNoViewNameShouldThrow()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new SmtpMailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            Assert.Throws<ArgumentNullException>(() => mailer.Email(null));
        }

        [Test]
        public void MultipartMessagesShouldRenderBothViews()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new SmtpMailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            RazorEmailResult email = mailer.Email("MultipartNoModel");
            string textBody = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd().Trim();
            string htmlBody = new StreamReader(email.Mail.AlternateViews[1].ContentStream).ReadToEnd().Trim();

            Assert.AreEqual("Testing multipart.", textBody);
            Assert.AreEqual("<p>Testing multipart.</p>", htmlBody);
        }

        [Test]
        public void PassingAMailSenderShouldWork()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new SmtpMailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);
            RazorEmailResult email = mailer.Email("TextViewNoModel");

            Assert.AreSame(mockSender, mailer.MailSender);
            Assert.AreSame(mockSender, email.MailSender);
        }

        [Test]
        public void PassingAModelShouldWork()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new SmtpMailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);
            var model = new TestModel
            {
                Name = "Foo"
            };

            RazorEmailResult email = mailer.Email("TextViewWithModel", model);
            string body = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd().Trim();

            Assert.AreEqual("Your name is:  Foo", body);
        }

        [Test]
        public void RazorViewWithNoModelShouldRenderProperly()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new SmtpMailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            RazorEmailResult email = mailer.Email("TextViewNoModel");
            string body = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd().Trim();

            Assert.AreEqual("This is a test", body);
        }

        [Test]
        public void WhiteSpaceShouldBeIncludedWhenRequired()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new SmtpMailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            RazorEmailResult email = mailer.Email("WhitespaceTrimTest", false);
            string body = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd();

            Assert.True(body.StartsWith(Environment.NewLine));
            Assert.True(body.EndsWith(Environment.NewLine));
        }

        [Test]
        public void WhiteSpaceShouldBeTrimmedWhenRequired()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new SmtpMailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            RazorEmailResult email = mailer.Email("WhitespaceTrimTest", trimBody: true);
            string body = new StreamReader(email.Mail.AlternateViews[0].ContentStream).ReadToEnd();

            Assert.AreEqual("This thing has leading and trailing whitespace.", body);
        }
    }
}