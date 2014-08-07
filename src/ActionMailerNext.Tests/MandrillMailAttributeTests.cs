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
using ActionMailerNext.Implementations.Mandrill;
using NUnit.Framework;

namespace ActionMailerNext.Tests {
    [TestFixture]
    public class MandrillMailAttributeTests {
        [Test]
        public void GeneratePropsectiveEmailMessageShouldSetCorrectMessageProperties()
        {
            var mailer = new MandrillMailAttributes();
            mailer.To.Add(new MailAddress("test@test.com"));
            mailer.From = new MailAddress("no-reply@mysite.com");
            mailer.Subject = "test subject";
            mailer.Bcc.Add(new MailAddress("test-bcc@test.com"));
            mailer.Headers.Add("X-No-Spam", "True");

            var logoAttachmentBytes = File.ReadAllBytes(Path.Combine(Assembly.GetExecutingAssembly().FullName, "..", "..", "..", "SampleData",
                    "logo.png"));
            mailer.Attachments["logo.png"] = logoAttachmentBytes;

            var result = mailer.GenerateProspectiveMailMessage();
            var attachment = result.attachments.First();
            var attachmentBytes = Convert.FromBase64String(attachment.content);

            Assert.AreEqual("test@test.com", result.to.First().email);
            Assert.AreEqual("no-reply@mysite.com", result.from_email);
            Assert.AreEqual("test-bcc@test.com", result.bcc_address);
            Assert.AreEqual("test subject", result.subject);
            Assert.AreEqual("True", result.headers["X-No-Spam"]);
            Assert.AreEqual("logo.png", attachment.name);
            Assert.AreEqual("image/png", attachment.type);


            Assert.True(attachmentBytes.SequenceEqual(logoAttachmentBytes));
        }
    }
}
