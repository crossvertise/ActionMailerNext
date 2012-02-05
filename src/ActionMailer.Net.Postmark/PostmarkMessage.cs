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
using System.Collections.Generic;

namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// A simple POCO that represents a message to be sent via postmark.
    /// </summary>
    [Serializable]
    public class PostmarkMessage {
        /// <summary>
        /// Who the message is from.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// A comma-separated list of addresses to send the message to.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// A comma-separated list of addresses to carbon-copy the message to.
        /// </summary>
        public string Cc { get; set; }

        /// <summary>
        /// A comma-separated list of addresses to blind carbon-copy the message to.
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// The subject of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// A special tag that you can specify for the message within the Postmark system.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The HTML body of the message.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// The plain text body of the message.
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// A comma-separated list of addresses that should be used for replies.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// A list of custom headers to attach to the message.
        /// </summary>
        public List<PostmarkHeader> Headers { get; set; }

        /// <summary>
        /// Creates a new PostmarkMessage to use when sending mail via Postmark.
        /// </summary>
        public PostmarkMessage() {
            Headers = new List<PostmarkHeader>();
        }
    }
}
