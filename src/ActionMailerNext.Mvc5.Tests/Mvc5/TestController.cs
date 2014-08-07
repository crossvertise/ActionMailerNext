using System.Web.Mvc;

namespace ActionMailerNext.Mvc5.Tests.Mvc5
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