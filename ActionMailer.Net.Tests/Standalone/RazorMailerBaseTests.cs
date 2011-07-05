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

using Xunit;

namespace ActionMailer.Net.Tests.Standalone {
    public class RazorMailerBaseTests {
        [Fact]
        public void EmailMethodShouldSetPropertiesOnMailMessage() {
            var mailer = new TestMailerBase();
            mailer.To.Add("test@test.com");
            mailer.From = "no-reply@mysite.com";
            mailer.Subject = "test subject";
            mailer.CC.Add("test-cc@test.com");
            mailer.BCC.Add("test-bcc@test.com");
            mailer.ReplyTo.Add("test-reply-to@test.com");
            mailer.Headers.Add("X-No-Spam", "True");

            var result = mailer.Email("TextViewNoModel");

            Assert.Equal("test@test.com", result.Mail.To[0].Address);
            Assert.Equal("no-reply@mysite.com", result.Mail.From.Address);
            Assert.Equal("test subject", result.Mail.Subject);
            Assert.Equal("test-cc@test.com", result.Mail.CC[0].Address);
            Assert.Equal("test-bcc@test.com", result.Mail.Bcc[0].Address);
            Assert.Equal("test-reply-to@test.com", result.Mail.ReplyToList[0].Address);
            Assert.Equal("True", result.Mail.Headers["X-No-Spam"]);
        }
    }
}
