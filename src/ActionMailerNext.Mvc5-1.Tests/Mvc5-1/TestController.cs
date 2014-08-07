using System.Web.Mvc;
using ActionMailerNext.Mvc5_1;

namespace ActionMailer.Net.Mvc5_1.Tests.Mvc5_1
{
    public class TestController : Controller
    {
        public string TestAction()
        {
            EmailResult email =
                new TestMailController {HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null)}.TestMail();
            return email.ViewName;
        }
    }
}