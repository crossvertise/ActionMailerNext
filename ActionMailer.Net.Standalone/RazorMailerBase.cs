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

using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ActionMailer.Net.Standalone {
    /// <summary>
    /// This is a standalone MailerBase that relies on RazorEngine to generate emails.
    /// </summary>
    public class RazorMailerBase : IMailerBase {
        /// <summary>
        /// A string representation of who this mail should be from.  Could be
        /// your name and email address or just an email address by itself.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// The subject line of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// A collection of addresses this email should be sent to.
        /// </summary>
        public IList<string> To { get; private set; }

        /// <summary>
        /// A collection of addresses that should be CC'ed.
        /// </summary>
        public IList<string> CC { get; private set; }

        /// <summary>
        /// A collection of addresses that should be BCC'ed.
        /// </summary>
        public IList<string> BCC { get; private set; }

        /// <summary>
        /// A collection of addresses that should be listed in Reply-To header.
        /// </summary>
        public IList<string> ReplyTo { get; private set; }

        /// <summary>
        /// Any custom headers (name and value) that should be placed on the message.
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Gets or sets the default message encoding when delivering mail.
        /// </summary>
        public Encoding MessageEncoding { get; set; }

        /// <summary>
        /// Any attachments you wish to add.  The key of this collection is what
        /// the file should be named.  The value is should represent the binary bytes
        /// of the file.
        /// </summary>
        /// <example>
        /// Attachments["picture.jpg"] = File.ReadAllBytes(@"C:\picture.jpg");
        /// </example>
        public AttachmentCollection Attachments { get; private set; }

        /// <summary>
        /// The underlying IMailSender to use for outgoing messages.
        /// </summary>
        public IMailSender MailSender { get; set; }

        /// <summary>
        /// This method is called after each mail is sent.
        /// </summary>
        /// <param name="mail">The mail that was sent.</param>
        protected virtual void OnMailSent(MailMessage mail) { }

        /// <summary>
        /// This method is called before each mail is sent
        /// </summary>
        /// <param name="context">A simple context containing the mail
        /// and a boolean value that can be toggled to prevent this
        /// mail from being sent.</param>
        protected virtual void OnMailSending(MailSendingContext context) { }

        void IMailInterceptor.OnMailSending(MailSendingContext context) {
            OnMailSending(context);
        }

        void IMailInterceptor.OnMailSent(MailMessage mail) {
            OnMailSent(mail);
        }
    }
}
