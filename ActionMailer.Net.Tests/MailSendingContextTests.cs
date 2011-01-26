using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Net.Mail;

namespace ActionMailer.Net.Tests {
    public class MailSendingContextTests {
        [Fact]
        public void MailContextConstructorSetsUpObjectProperly() {
            var mail = new MailMessage("no-reply@test.com", "test@test.com");
            
            var context = new MailSendingContext(mail);

            Assert.Equal(mail, context.Mail);
            Assert.False(context.Cancel);
        }
    }
}
