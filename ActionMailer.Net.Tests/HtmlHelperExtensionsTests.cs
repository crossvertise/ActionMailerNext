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

using System.Web.Mvc;
using HtmlAgilityPack;
using Moq;
using Xunit;

namespace ActionMailer.Net.Tests {
    public class HtmlHelperExtensionsTests {
        private readonly HtmlHelper _helper;

        public HtmlHelperExtensionsTests() {
            var context = new Mock<ViewContext>();
            var container = new Mock<IViewDataContainer>();

            _helper  = new HtmlHelper(context.Object, container.Object);
        }

        [Fact]
        public void InlineImageWithContentId() {
            var generatedImageTag = _helper.InlineImage("test.png").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            var imgTag = doc.DocumentNode.FirstChild;

            Assert.Equal("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.Equal("cid:test.png", imgTag.Attributes["src"].Value);
        }

        [Fact]
        public void InlineImageWithContentIdAndHtmlAttributes() {
            var generatedImageTag = _helper.InlineImage("test.png", new {@class = "test"}).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            var imgTag = doc.DocumentNode.FirstChild;

            Assert.Equal("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.Equal("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["class"] != null);
            Assert.Equal("test", imgTag.Attributes["class"].Value);
        }

        [Fact]
        public void InlineImageWithContentIdAndAlt() {
            var generatedImageTag = _helper.InlineImage("test.png", "alt message").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            var imgTag = doc.DocumentNode.FirstChild;

            Assert.Equal("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.Equal("cid:test.png", imgTag.Attributes["src"].Value);
        }
    }
}
