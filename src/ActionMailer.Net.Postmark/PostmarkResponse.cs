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

namespace ActionMailer.Net.Postmark {
    /// <summary>
    /// Represents the response received back from Postmark.
    /// </summary>
    public class PostmarkResponse {
        /// <summary>
        /// The error code.  0 indicates no error.  Any other number is an error.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// The message received from Postmark.  Can contain details for errors.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The Postmark-generated ID for the message that was sent.
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// The date and time when the message was submitted to Postmark.
        /// </summary>
        public DateTime SubmittedAt { get; set; }

        /// <summary>
        /// The recepient of the message.
        /// </summary>
        public string To { get; set; }
    }
}
