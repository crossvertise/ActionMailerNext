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

using System.Web.Mvc;
using ActionMailer.Net.Mvc;
using FakeItEasy;
using HtmlAgilityPack;
using Xunit;

namespace ActionMailer.Net.Tests.Mvc {
    public class HtmlHelperExtensionsTests {
        private readonly HtmlHelper _helper;

        public HtmlHelperExtensionsTests() {
            var context = A.Fake<ViewContext>();
            var container = A.Fake<IViewDataContainer>();

            _helper  = new HtmlHelper(context, container);
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

            Assert.True(imgTag.Attributes["alt"] != null);
            Assert.Equal("alt message", imgTag.Attributes["alt"].Value);
        }

        [Fact]
        public void InlineImageWithContentIdAndAltAndHtmlAttributes() {
            var generatedImageTag = _helper.InlineImage("test.png", "alt message", new { @class = "test" }).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            var imgTag = doc.DocumentNode.FirstChild;

            Assert.Equal("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.Equal("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["class"] != null);
            Assert.Equal("test", imgTag.Attributes["class"].Value);

            Assert.True(imgTag.Attributes["alt"] != null);
            Assert.Equal("alt message", imgTag.Attributes["alt"].Value);
        }

        [Fact]
        public void InlineImageWithContentIdAndAltAndId() {
            var generatedImageTag = _helper.InlineImage("test.png", "alt message", "image-id").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            var imgTag = doc.DocumentNode.FirstChild;

            Assert.Equal("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.Equal("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["alt"] != null);
            Assert.Equal("alt message", imgTag.Attributes["alt"].Value);

            Assert.True(imgTag.Attributes["id"] != null);
            Assert.Equal("image-id", imgTag.Attributes["id"].Value);
        }

        [Fact]
        public void InlineImagewithContentIdAndAltAndIdAndHtmlAttributes() {
            var generatedImageTag = _helper.InlineImage("test.png", "alt message", "image-id", new {@class = "test"}).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            var imgTag = doc.DocumentNode.FirstChild;

            Assert.Equal("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.Equal("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["class"] != null);
            Assert.Equal("test", imgTag.Attributes["class"].Value);

            Assert.True(imgTag.Attributes["alt"] != null);
            Assert.Equal("alt message", imgTag.Attributes["alt"].Value);

            Assert.True(imgTag.Attributes["id"] != null);
            Assert.Equal("image-id", imgTag.Attributes["id"].Value);
        }

        [Fact]
        public void InlineCSSWithContentId() {
            var generatedLinkTag = _helper.InlineCSS("test.css").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedLinkTag);
            var linkTag = doc.DocumentNode.FirstChild;

            Assert.Equal("link", linkTag.Name);
            Assert.Equal("stylesheet", linkTag.Attributes["rel"].Value);
            Assert.Equal("text/css", linkTag.Attributes["type"].Value);

            Assert.True(linkTag.Attributes["href"] != null);
            Assert.Equal("cid:test.css", linkTag.Attributes["href"].Value);
        }

        [Fact]
        public void InlineCSSWithContentIdAndHtmlAttributes() {
            var generatedLinkTag = _helper.InlineCSS("test.css", new {trunk = "junk"}).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedLinkTag);
            var linkTag = doc.DocumentNode.FirstChild;

            Assert.Equal("link", linkTag.Name);
            Assert.Equal("stylesheet", linkTag.Attributes["rel"].Value);
            Assert.Equal("text/css", linkTag.Attributes["type"].Value);

            Assert.True(linkTag.Attributes["href"] != null);
            Assert.Equal("cid:test.css", linkTag.Attributes["href"].Value);

            Assert.True(linkTag.Attributes["trunk"] != null);
            Assert.Equal("junk", linkTag.Attributes["trunk"].Value);
        }

        [Fact]
        public void InlineCSSWithContentIdAndMedia() {
            var generatedLinkTag = _helper.InlineCSS("test.css", "screen").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedLinkTag);
            var linkTag = doc.DocumentNode.FirstChild;

            Assert.Equal("link", linkTag.Name);
            Assert.Equal("stylesheet", linkTag.Attributes["rel"].Value);
            Assert.Equal("text/css", linkTag.Attributes["type"].Value);

            Assert.True(linkTag.Attributes["href"] != null);
            Assert.Equal("cid:test.css", linkTag.Attributes["href"].Value);

            Assert.True(linkTag.Attributes["media"] != null);
            Assert.Equal("screen", linkTag.Attributes["media"].Value);
        }

        [Fact]
        public void InlineCSSWithContentIdAndMediaAndHtmlAttributes() {
            var generatedLinkTag = _helper.InlineCSS("test.css", "screen", new { trunk = "junk" }).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedLinkTag);
            var linkTag = doc.DocumentNode.FirstChild;

            Assert.Equal("link", linkTag.Name);
            Assert.Equal("stylesheet", linkTag.Attributes["rel"].Value);
            Assert.Equal("text/css", linkTag.Attributes["type"].Value);

            Assert.True(linkTag.Attributes["href"] != null);
            Assert.Equal("cid:test.css", linkTag.Attributes["href"].Value);

            Assert.True(linkTag.Attributes["media"] != null);
            Assert.Equal("screen", linkTag.Attributes["media"].Value);

            Assert.True(linkTag.Attributes["trunk"] != null);
            Assert.Equal("junk", linkTag.Attributes["trunk"].Value);
        }
    }
}
