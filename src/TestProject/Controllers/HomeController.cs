using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ActionMailer.Net.Mvc5_2;

namespace TestProject.Controllers
{
    public class HomeController : MailerBase
    {
        private DateTime _startTime;
        private DateTime _endTime;
        public EmailResult TestMandrillEmail()
        {
            SetMailMethod(MailMethod.Mandrill);

            MailAttributes.From = new MailAddress("a.kamel@crossvertise.com", "Mr. Kamel");
            MailAttributes.To.Add(new MailAddress("dodohani@gmail.com"));
            MailAttributes.Subject = "Mandrill Version";

            var attachmentBytes = System.IO.File.ReadAllBytes("C:/temp/SampleData/attachment.zip");
            MailAttributes.Attachments.Add("attachment.zip", new Attachment(new MemoryStream(attachmentBytes), "attachment.zip"));

            return Email("View");
        }

        public EmailResult TestSMTPEmail()
        {
            SetMailMethod(MailMethod.SMTP);

            MailAttributes.From = new MailAddress("a.kamel@crossvertise.com", "Mr. Kamel");
            MailAttributes.To.Add(new MailAddress("dodohani@gmail.com"));
            MailAttributes.Subject = "SMTP Version";
            MailAttributes.Priority = MailPriority.High;

            var attachmentBytes = System.IO.File.ReadAllBytes("C:/temp/SampleData/attachment.zip");

            MailAttributes.Attachments.Add("attachment.zip", new Attachment(new MemoryStream(attachmentBytes), "attachment.zip"));

            return Email("View");
        }

        public async Task TestAsync()
        {
            await TestMandrillEmail().DeliverAsync();
        }

        public void TestSync()
        {
         /*   _startTime = DateTime.Now;
            TestMandrillEmail().Deliver();
            _endTime = DateTime.Now;

            var mandrilltimeSpan = _endTime - _startTime;
            */
            _startTime = DateTime.Now;
            TestSMTPEmail().Deliver();
            _endTime = DateTime.Now;

            var smtptimeSpan = _endTime - _startTime;


            //Debug.WriteLine("Time Needed (Mandrill) : \t {0}", mandrilltimeSpan);
            Debug.WriteLine("Time Needed (SMTP) : \t {0}", smtptimeSpan);
        }
    }
}