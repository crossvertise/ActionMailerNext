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

using System.Net.Mail;

namespace ActionMailer.Net {
    /// <summary>
    /// A special context object used by the OnMailSending() method
    /// to allow you to inspect the underlying MailMessage before it
    /// is sent, or prevent it from being sent altogether.
    /// </summary>
    public class MailSendingContext {
        /// <summary>
        /// The generated mail message that is being sent.
        /// </summary>
        public readonly MailMessage Mail;

        /// <summary>
        /// A special flag that you can toggle to prevent this mail
        /// from being sent.
        /// </summary>
        public readonly bool Cancel;

        /// <summary>
        /// Returns a populated context to be used for the OnMailSending()
        /// method in MailerBase.
        /// </summary>
        /// <param name="mail">The message you wish to wrap within this context.</param>
        public MailSendingContext(MailMessage mail) {
            Mail = mail;
            Cancel = false;
        }
    }
}
