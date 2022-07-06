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
namespace ActionMailerNext.Tests.Standalone
{
    using System;

    using ActionMailerNext.Standalone;
    using ActionMailerNext.Standalone.Interfaces;

    using FakeItEasy;

    using NUnit.Framework;

    public class RazorEmailResultTests
    {
        [Test]
        public void ConstructorWithNullMailMessageThrows()
        {
            var mockTemplateService = A.Fake<ITemplateService>();

            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new HBSEmailResult(null, "View", null, "_Layout", "Path", mockTemplateService,
                        null);
                });
        }

        [Test]
        public void ConstructorWithNullViewNameThrows()
        {
            var mockTemplateService = A.Fake<ITemplateService>();

            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new HBSEmailResult(new MailAttributes(), null, null, "_Layout", "Path",
                        mockTemplateService, null);
                });
        }

        [Test]
        public void ConstructorWithNullTemplateServiceThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new HBSEmailResult(new MailAttributes(), "View", null, "_Layout", "Path",
                        null, null);
                });
        }
    }
}