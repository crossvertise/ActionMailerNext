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
using System.Reflection;
using Xunit;

namespace ActionMailer.Net.Tests {
    public class MailerBaseExtensionsTests {
        [Fact]
        public void GenerateMailShouldSetCorrectMessageProperties() {
            var mailer = new StubMailerBase();
            mailer.To.Add("test@test.com");
            mailer.From = "no-reply@mysite.com";
            mailer.Subject = "test subject";
            mailer.CC.Add("test-cc@test.com");
            mailer.BCC.Add("test-bcc@test.com");
            mailer.ReplyTo.Add("test-reply-to@test.com");
            mailer.Headers.Add("X-No-Spam", "True");
            var logoBytes = File.ReadAllBytes(Path.Combine(Assembly.GetExecutingAssembly().FullName, "..", "..", "..", "SampleData", "logo.png"));
            mailer.Attachments.Add("logo.png", logoBytes);
            mailer.Attachments.Inline.Add("logo-inline.png", logoBytes);

            var result = mailer.GenerateMail();
            var attachment = result.Attachments[0];
            var inlineAttachment = result.Attachments[1];
            var attachmentBytes = new byte[attachment.ContentStream.Length];
            var inlineAttachmentBytes = new byte[inlineAttachment.ContentStream.Length];
            attachment.ContentStream.Read(attachmentBytes, 0, Convert.ToInt32(attachment.ContentStream.Length));
            inlineAttachment.ContentStream.Read(inlineAttachmentBytes, 0, Convert.ToInt32(inlineAttachment.ContentStream.Length));

            Assert.Equal("test@test.com", result.To[0].Address);
            Assert.Equal("no-reply@mysite.com", result.From.Address);
            Assert.Equal("test subject", result.Subject);
            Assert.Equal("test-cc@test.com", result.CC[0].Address);
            Assert.Equal("test-bcc@test.com", result.Bcc[0].Address);
            Assert.Equal("test-reply-to@test.com", result.ReplyToList[0].Address);
            Assert.Equal("True", result.Headers["X-No-Spam"]);
            Assert.Equal("logo.png", attachment.ContentId);
            Assert.Equal("logo-inline.png", inlineAttachment.ContentId);
            Assert.Equal("image/png", attachment.ContentType.MediaType);
            Assert.Equal("multipart/related", inlineAttachment.ContentType.MediaType);
            Assert.True(inlineAttachment.ContentDisposition.Inline);
            Assert.True(attachmentBytes.SequenceEqual(logoBytes));
            Assert.True(inlineAttachmentBytes.SequenceEqual(logoBytes));
        }
    }
}
