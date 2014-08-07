using System.Web.Mvc;
using ActionMailerNext.Mvc5_2;

namespace ActionMailer.Net.Mvc5_2.Tests.Mvc5_2
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