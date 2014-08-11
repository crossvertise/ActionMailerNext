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
using FakeItEasy;
using HtmlAgilityPack;
using NUnit.Framework;

namespace ActionMailerNext.Mvc5_1.Tests
{
    [TestFixture]
    public class HtmlHelperExtensionsTests
    {
        private readonly HtmlHelper _helper;

        public HtmlHelperExtensionsTests()
        {
            var context = A.Fake<ViewContext>();
            var container = A.Fake<IViewDataContainer>();

            _helper = new HtmlHelper(context, container);
        }

        [Test]
        public void InlineCSSWithContentId()
        {
            string generatedLinkTag = _helper.InlineCSS("test.css").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedLinkTag);
            HtmlNode linkTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("link", linkTag.Name);
            Assert.AreEqual("stylesheet", linkTag.Attributes["rel"].Value);
            Assert.AreEqual("text/css", linkTag.Attributes["type"].Value);

            Assert.True(linkTag.Attributes["href"] != null);
            Assert.AreEqual("cid:test.css", linkTag.Attributes["href"].Value);
        }

        [Test]
        public void InlineCSSWithContentIdAndHtmlAttributes()
        {
            string generatedLinkTag = _helper.InlineCSS("test.css", new {trunk = "junk"}).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedLinkTag);
            HtmlNode linkTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("link", linkTag.Name);
            Assert.AreEqual("stylesheet", linkTag.Attributes["rel"].Value);
            Assert.AreEqual("text/css", linkTag.Attributes["type"].Value);

            Assert.True(linkTag.Attributes["href"] != null);
            Assert.AreEqual("cid:test.css", linkTag.Attributes["href"].Value);

            Assert.True(linkTag.Attributes["trunk"] != null);
            Assert.AreEqual("junk", linkTag.Attributes["trunk"].Value);
        }

        [Test]
        public void InlineCSSWithContentIdAndMedia()
        {
            string generatedLinkTag = _helper.InlineCSS("test.css", "screen").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedLinkTag);
            HtmlNode linkTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("link", linkTag.Name);
            Assert.AreEqual("stylesheet", linkTag.Attributes["rel"].Value);
            Assert.AreEqual("text/css", linkTag.Attributes["type"].Value);

            Assert.True(linkTag.Attributes["href"] != null);
            Assert.AreEqual("cid:test.css", linkTag.Attributes["href"].Value);

            Assert.True(linkTag.Attributes["media"] != null);
            Assert.AreEqual("screen", linkTag.Attributes["media"].Value);
        }

        [Test]
        public void InlineCSSWithContentIdAndMediaAndHtmlAttributes()
        {
            string generatedLinkTag = _helper.InlineCSS("test.css", "screen", new {trunk = "junk"}).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedLinkTag);
            HtmlNode linkTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("link", linkTag.Name);
            Assert.AreEqual("stylesheet", linkTag.Attributes["rel"].Value);
            Assert.AreEqual("text/css", linkTag.Attributes["type"].Value);

            Assert.True(linkTag.Attributes["href"] != null);
            Assert.AreEqual("cid:test.css", linkTag.Attributes["href"].Value);

            Assert.True(linkTag.Attributes["media"] != null);
            Assert.AreEqual("screen", linkTag.Attributes["media"].Value);

            Assert.True(linkTag.Attributes["trunk"] != null);
            Assert.AreEqual("junk", linkTag.Attributes["trunk"].Value);
        }

        [Test]
        public void InlineImageWithContentId()
        {
            string generatedImageTag = _helper.InlineImage("test.png").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            HtmlNode imgTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.AreEqual("cid:test.png", imgTag.Attributes["src"].Value);
        }

        [Test]
        public void InlineImageWithContentIdAndAlt()
        {
            string generatedImageTag = _helper.InlineImage("test.png", "alt message").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            HtmlNode imgTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.AreEqual("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["alt"] != null);
            Assert.AreEqual("alt message", imgTag.Attributes["alt"].Value);
        }

        [Test]
        public void InlineImageWithContentIdAndAltAndHtmlAttributes()
        {
            string generatedImageTag = _helper.InlineImage("test.png", "alt message", new {@class = "test"}).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            HtmlNode imgTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.AreEqual("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["class"] != null);
            Assert.AreEqual("test", imgTag.Attributes["class"].Value);

            Assert.True(imgTag.Attributes["alt"] != null);
            Assert.AreEqual("alt message", imgTag.Attributes["alt"].Value);
        }

        [Test]
        public void InlineImageWithContentIdAndAltAndId()
        {
            string generatedImageTag = _helper.InlineImage("test.png", "alt message", "image-id").ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            HtmlNode imgTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.AreEqual("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["alt"] != null);
            Assert.AreEqual("alt message", imgTag.Attributes["alt"].Value);

            Assert.True(imgTag.Attributes["id"] != null);
            Assert.AreEqual("image-id", imgTag.Attributes["id"].Value);
        }

        [Test]
        public void InlineImageWithContentIdAndHtmlAttributes()
        {
            string generatedImageTag = _helper.InlineImage("test.png", new {@class = "test"}).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            HtmlNode imgTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.AreEqual("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["class"] != null);
            Assert.AreEqual("test", imgTag.Attributes["class"].Value);
        }

        [Test]
        public void InlineImagewithContentIdAndAltAndIdAndHtmlAttributes()
        {
            string generatedImageTag =
                _helper.InlineImage("test.png", "alt message", "image-id", new {@class = "test"}).ToString();
            var doc = new HtmlDocument();
            doc.LoadHtml(generatedImageTag);
            HtmlNode imgTag = doc.DocumentNode.FirstChild;

            Assert.AreEqual("img", imgTag.Name);
            Assert.True(imgTag.Attributes["src"] != null);
            Assert.AreEqual("cid:test.png", imgTag.Attributes["src"].Value);

            Assert.True(imgTag.Attributes["class"] != null);
            Assert.AreEqual("test", imgTag.Attributes["class"].Value);

            Assert.True(imgTag.Attributes["alt"] != null);
            Assert.AreEqual("alt message", imgTag.Attributes["alt"].Value);

            Assert.True(imgTag.Attributes["id"] != null);
            Assert.AreEqual("image-id", imgTag.Attributes["id"].Value);
        }
    }
}