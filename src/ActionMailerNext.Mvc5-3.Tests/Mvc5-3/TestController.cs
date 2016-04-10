using System.Web.Mvc;

namespace ActionMailerNext.Mvc5_3.Tests
{
    public class TestController : Controller
    {
        public string TestAction()
        {
            var email =
                new TestMailController {HttpContextBase = MvcHelper.GetHttpContext("/app/", null, null)}.TestMail();
            return email.ViewName;
        }
    }
}