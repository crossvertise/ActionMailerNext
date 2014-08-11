using System.Web.Mvc;

namespace ActionMailerNext.Mvc5_1.Tests
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