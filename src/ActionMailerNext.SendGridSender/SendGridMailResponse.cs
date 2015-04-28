using System;

namespace ActionMailerNext.SendGridSender
{
    using ActionMailerNext.Interfaces;

    public class SendGridMailResponse : IMailResponse
    {
        public string Email { get; set; }

        public string Status { get; set; }

        public override string ToString()
        {
            return String.Format("Email : {0}\nStatus : {1}", Email, Status);
        }
    }
}