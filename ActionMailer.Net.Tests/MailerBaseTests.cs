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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Moq;
using Xunit;

namespace ActionMailer.Net.Tests {
    public class MailerBaseTests {
        [Fact]
        public void EmailMethodShouldSetPropertiesOnMailMessage() {
            var mailer = new MailerBase();
            ViewEngines.Engines.Add(new TestViewEngine());
            mailer.HttpContextBase = new EmptyHttpContextBase();
            mailer.To.Add("test@test.com");
            mailer.From = "no-reply@mysite.com";
            mailer.Subject = "test subject";
            mailer.CC.Add("test-cc@test.com");
            mailer.BCC.Add("test-bcc@test.com");
            mailer.ReplyTo.Add("test-reply-to@test.com");
            mailer.Headers.Add("X-No-Spam", "True");

            var result = mailer.Email();

            Assert.Equal("test@test.com", result.Mail.To[0].Address);
            Assert.Equal("no-reply@mysite.com", result.Mail.From.Address);
            Assert.Equal("test subject", result.Mail.Subject);
            Assert.Equal("test-cc@test.com", result.Mail.CC[0].Address);
            Assert.Equal("test-bcc@test.com", result.Mail.Bcc[0].Address);
            Assert.Equal("test-reply-to@test.com", result.Mail.ReplyToList[0].Address);
            Assert.Equal("True", result.Mail.Headers["X-No-Spam"]);
        }

        [Fact]
        public void PassingAMailSenderShouldWork() {
            var mockSender = new Mock<IMailSender>();
            
            var mailer = new MailerBase(mockSender.Object);

            Assert.Equal(mockSender.Object, mailer.MailSender);
        }

        [Fact]
        public void EmailMethodShouldRenderViewAsMessageBody() {
            var mailer = new MailerBase();
            ViewEngines.Engines.Add(new TestViewEngine());
            mailer.HttpContextBase = new EmptyHttpContextBase();
            mailer.From = "no-reply@mysite.com";

            // there's no need to test the built-in view engines.
            // this test just ensures that our Email() method actually
            // populates the mail body properly.
            var result = mailer.Email();

            Assert.Equal("TestView", result.Mail.Body);
        }

        [Fact]
        public void AttachmentsShouldAttachProperly() {
            var mailer = new MailerBase();
            ViewEngines.Engines.Add(new TestViewEngine());
            mailer.HttpContextBase = new EmptyHttpContextBase();
            mailer.From = "no-reply@mysite.com";
            var imagePath = Path.Combine(Assembly.GetExecutingAssembly().FullName, "..", "..", "..", "SampleData", "logo.png");
            var imageBytes = File.ReadAllBytes(imagePath);
            mailer.Attachments["logo.png"] = imageBytes;
            mailer.Attachments.Inline["logo-inline.png"] = imageBytes;

            var result = mailer.Email();
            var attachment = result.Mail.Attachments[0];
            var inlineAttachment = result.Mail.Attachments[1];
            byte[] attachmentBytes = new byte[attachment.ContentStream.Length];
            attachment.ContentStream.Read(attachmentBytes, 0, Convert.ToInt32(attachment.ContentStream.Length));

            Assert.Equal("logo.png", attachment.Name);
            Assert.Equal("image/png", attachment.ContentType.MediaType);
            Assert.True(attachmentBytes.SequenceEqual(imageBytes));
            Assert.True(inlineAttachment.ContentDisposition.Inline);
        }
    }
}
