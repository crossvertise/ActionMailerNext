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
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Mvc5_1.Tests.Areas.TestArea.Controllers;
using FakeItEasy;
using NUnit.Framework;

namespace ActionMailerNext.Mvc5_1.Tests
{
    [TestFixture]
    public class MailerBaseTests
    {
        [Test]
        public void AreasAreDetectedProperly()
        {
            var rd = new RouteData();
            rd.Values.Add("area", "TestArea");
            var mailer = new MailController();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TextViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);

            mailer.TestEmail();

            Assert.NotNull(mailer.ControllerContext.RouteData.DataTokens["area"]);
            Assert.AreEqual("TestArea", mailer.ControllerContext.RouteData.DataTokens["area"]);
        }

        [Test]
        public void EmailMethodShouldAllowMultipleViews()
        {
            var mailer = new TestMailerBase();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MultipartViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");

            // there's no need to test the built-in view engines.
            // this test just ensures that our Email() method actually
            // populates the mail body properly.
            EmailResult result = mailer.Email("TestView");

            Assert.AreEqual(2, result.MailAttributes.AlternateViews.Count());

            var textReader = new StreamReader(result.MailAttributes.AlternateViews[0].ContentStream);
            string textBody = textReader.ReadToEnd();
            StringAssert.Contains("TextView", textBody);
            Assert.AreEqual("text/plain", result.MailAttributes.AlternateViews[0].ContentType.MediaType);

            var htmlReader = new StreamReader(result.MailAttributes.AlternateViews[1].ContentStream);
            string htmlBody = htmlReader.ReadToEnd();
            StringAssert.Contains("HtmlView", htmlBody);
            Assert.AreEqual("text/html", result.MailAttributes.AlternateViews[1].ContentType.MediaType);
        }

        [Test]
        public void EmailMethodShouldRenderViewAsMessageBody()
        {
            var mailer = new TestMailerBase();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TextViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");

            // there's no need to test the built-in view engines.
            // this test just ensures that our Email() method actually
            // populates the mail body properly.
            EmailResult result = mailer.Email("TestView");
            var reader = new StreamReader(result.MailAttributes.AlternateViews[0].ContentStream);
            string body = reader.ReadToEnd().Trim();

            Assert.AreEqual("TextView", body);
        }

        [Test]
        public void MasterNameShouldBePassedProperly()
        {
            var mailer = new TestMailController();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TextViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);

            EmailResult email = mailer.TestMaster();

            Assert.AreEqual("TestMaster", email.MasterName);
        }

        [Test]
        public void MessageEncodingOverrideShouldWork()
        {
            var mailer = new TestMailerBase();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new UTF8ViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");
            mailer.MailAttributes.MessageEncoding = Encoding.UTF8;

            EmailResult result = mailer.Email("TestView");
            var reader = new StreamReader(result.MailAttributes.AlternateViews[0].ContentStream);
            string body = reader.ReadToEnd();

            Assert.AreEqual(Encoding.UTF8, result.MessageEncoding);
            Assert.AreEqual("Umlauts are Über!", body);
        }

        [Test]
        public void ModelObjectShouldCopyToEmailResult()
        {
            var mailer = new TestMailerBase();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TextViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");

            object model = "12345";
            EmailResult result = mailer.Email("TestView", model);

            Assert.AreSame(model, result.ViewData.Model);
        }

        [Test]
        public void PassingAMailSenderShouldWork()
        {
            var mockSender = A.Fake<IMailSender>();
            var attributes = new MailAttributes();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TextViewEngine());

            var mailer = new TestMailerBase(attributes, mockSender);
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");
            EmailResult result = mailer.Email("TestView");

            Assert.AreSame(mockSender, mailer.MailSender);
            Assert.AreSame(mockSender, result.MailSender);
        }

        [Test]
        public void ViewBagDataShouldCopyToEmailResult()
        {
            var mailer = new TestMailerBase();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TextViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");

            mailer.ViewBag.Test = "12345";
            EmailResult result = mailer.Email("TestView");

            Assert.AreEqual("12345", result.ViewBag.Test);
        }

        [Test]
        public void ViewDataShouldCopyToEmailResult()
        {
            var mailer = new TestMailerBase();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TextViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");

            mailer.ViewData["foo"] = "bar";
            EmailResult result = mailer.Email("TestView");

            Assert.True(result.ViewData.ContainsKey("foo"));
            Assert.AreEqual("bar", result.ViewData["foo"]);
        }

        [Test]
        public void ViewNameShouldBePassedProperly()
        {
            var mailer = new TestMailController();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TextViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);

            EmailResult email = mailer.TestMail();

            Assert.AreEqual("TestView", email.ViewName);
        }

        [Test]
        public void ViewNameShouldBeRequiredWhenUsingCallingEmailMethod()
        {
            var mailer = new TestMailerBase();
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);

            Assert.Throws<ArgumentNullException>(() => mailer.Email(null));
        }

        [Test]
        public void WhiteSpaceShouldBeIncludedWhenRequired()
        {
            var mailer = new TestMailerBase();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new WhiteSpaceViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");

            // there's no need to test the built-in view engines.
            // this test just ensures that our Email() method actually
            // populates the mail body properly.
            EmailResult result = mailer.Email("WhiteSpaceView", trimBody: false);
            var reader = new StreamReader(result.MailAttributes.AlternateViews[0].ContentStream);
            string body = reader.ReadToEnd();

            Assert.True(body.StartsWith(Environment.NewLine));
            Assert.True(body.EndsWith(Environment.NewLine));
        }

        [Test]
        public void WhiteSpaceShouldBeTrimmedWhenRequired()
        {
            var mailer = new TestMailerBase();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new WhiteSpaceViewEngine());
            mailer.HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null);
            mailer.MailAttributes.From = new MailAddress("no-reply@mysite.com");

            // there's no need to test the built-in view engines.
            // this test just ensures that our Email() method actually
            // populates the mail body properly.
            EmailResult result = mailer.Email("WhiteSpaceView", trimBody: true);
            var reader = new StreamReader(result.MailAttributes.AlternateViews[0].ContentStream);
            string body = reader.ReadToEnd();

            Assert.AreEqual("This thing has leading and trailing whitespace.", body);
        }
    }
}