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
namespace ActionMailerNext.Tests.Standalone
{
    using System;
    using System.IO;

    using ActionMailerNext.Interfaces;

    using FakeItEasy;

    using NUnit.Framework;

    [TestFixture]
    public class RazorMailerBaseTests
    {
        [Test]
        public void EmailWithNoViewNameShouldThrow()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new MailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            Assert.Throws<ArgumentNullException>(() => mailer.Email(null));
        }

        [Test]
        public void MultipartMessagesShouldRenderBothViews()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new MailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            var email = mailer.Email("MultipartNoModel");
            var htmlBody = new StreamReader(email.MailAttributes.AlternateViews[0].ContentStream).ReadToEnd().Trim();

            Assert.AreEqual("<p>Testing multipart.</p>", htmlBody);
        }

        [Test]
        public void PassingAModelShouldWork()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new MailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);
            var model = new TestModel
            {
                Name = "Foo"
            };

            var email = mailer.Email("TextViewWithModel", model);
            var body = new StreamReader(email.MailAttributes.AlternateViews[0].ContentStream).ReadToEnd().Trim();

            Assert.AreEqual("Your name is:  Foo", body);
        }

        [Test]
        public void WhiteSpaceShouldBeIncludedWhenRequired()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new MailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            var email = mailer.Email("WhitespaceTrimTest", trimBody: false);
            var body = new StreamReader(email.MailAttributes.AlternateViews[0].ContentStream).ReadToEnd();

            Assert.True(char.IsWhiteSpace(body, 0));
            Assert.True(char.IsWhiteSpace(body, body.Length - 1));
        }

        [Test]
        public void WhiteSpaceShouldBeTrimmedWhenRequired()
        {
            var mockSender = A.Fake<IMailSender>();
            var attribute = new MailAttributes();
            var mailer = new TestMailerBase(attribute, mockSender);

            var email = mailer.Email("WhitespaceTrimTest", trimBody: true);
            var body = new StreamReader(email.MailAttributes.AlternateViews[0].ContentStream).ReadToEnd();

            Assert.AreEqual("This thing has leading and trailing whitespace.", body);
        }
    }
}