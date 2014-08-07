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
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using ActionMailerNext.Implementations.SMTP;
using NUnit.Framework;

namespace ActionMailerNext.Tests {
    [TestFixture]
    public class SMTPMailAttributeTests {
        [Test]
        public void GeneratePropsectiveEmailMessageShouldSetCorrectMessageProperties()
        {
            var mailer = new SmtpMailAttributes();
            mailer.To.Add(new MailAddress("test@test.com"));
            mailer.From = new MailAddress("no-reply@mysite.com");
            mailer.Subject = "test subject";
            mailer.Cc.Add(new MailAddress("test-cc@test.com"));
            mailer.Bcc.Add(new MailAddress("test-bcc@test.com"));
            mailer.ReplyTo.Add(new MailAddress("test-reply-to@test.com"));
            mailer.Headers.Add("X-No-Spam", "True");

            var logoAttachmentBytes = File.ReadAllBytes(Path.Combine(Assembly.GetExecutingAssembly().FullName, "..", "..", "..", "SampleData",
                    "logo.png"));
            mailer.Attachments["logo.png"] = logoAttachmentBytes;

            mailer.Attachments.Inline["logo-inline.png"] = logoAttachmentBytes;

            var result = mailer.GenerateProspectiveMailMessage();
            var attachment = result.Attachments[0];
            var inlineAttachment = result.Attachments[1];
            var attachmentBytes = new byte[attachment.ContentStream.Length];
            var inlineAttachmentBytes = new byte[inlineAttachment.ContentStream.Length];
            attachment.ContentStream.Read(attachmentBytes, 0, Convert.ToInt32(attachment.ContentStream.Length));
            inlineAttachment.ContentStream.Read(inlineAttachmentBytes, 0, Convert.ToInt32(inlineAttachment.ContentStream.Length));

            Assert.AreEqual("test@test.com", result.To[0].Address);
            Assert.AreEqual("no-reply@mysite.com", result.From.Address);
            Assert.AreEqual("test subject", result.Subject);
            Assert.AreEqual("test-cc@test.com", result.CC[0].Address);
            Assert.AreEqual("test-bcc@test.com", result.Bcc[0].Address);
            Assert.AreEqual("test-reply-to@test.com", result.ReplyToList[0].Address);
            Assert.AreEqual("True", result.Headers["X-No-Spam"]);
            Assert.AreEqual("logo.png", attachment.ContentId);
            Assert.AreEqual("logo-inline.png", inlineAttachment.ContentId);
            Assert.AreEqual("image/png", attachment.ContentType.MediaType);
            Assert.AreEqual("multipart/related", inlineAttachment.ContentType.MediaType);
            Assert.True(inlineAttachment.ContentDisposition.Inline);


            Assert.True(attachmentBytes.SequenceEqual(logoAttachmentBytes));
            Assert.True(inlineAttachmentBytes.SequenceEqual(logoAttachmentBytes));
        }
    }
}
