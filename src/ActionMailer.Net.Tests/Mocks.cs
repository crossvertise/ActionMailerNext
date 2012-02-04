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

using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using FakeItEasy;

namespace ActionMailer.Net.Tests {
    public class StubMailerBase : IMailerBase {
        public string From { get; set; }
        public string Subject { get; set; }
        public IList<string> To { get; private set; }
        public IList<string> CC { get; private set; }
        public IList<string> BCC { get; private set; }
        public IList<string> ReplyTo { get; private set; }
        public IDictionary<string, string> Headers { get; private set; }
        public Encoding MessageEncoding { get; set; }
        public AttachmentCollection Attachments { get; private set; }
        public IMailSender MailSender { get; set; }
        public void OnMailSending(MailSendingContext context) { }
        public void OnMailSent(MailMessage mail) { }

        public StubMailerBase() {
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
            ReplyTo = new List<string>();
            Headers = new Dictionary<string, string>();
            MessageEncoding = Encoding.Default;
            Attachments = new AttachmentCollection();
            MailSender = A.Fake<IMailSender>();
        }
    }
}
