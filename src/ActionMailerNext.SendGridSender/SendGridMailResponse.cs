using System;

namespace ActionMailerNext.SendGridSender
{
    using ActionMailerNext.Implementations.SMTP;
    using ActionMailerNext.Interfaces;

    public class SendGridMailResponse : IMailResponse
    {
        public string Email { get; set; }

        public string Status { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        public override string ToString()
        {
            return String.Format("Email : {0}\nStatus : {1}", Email, Status);
        }
        public static DeliveryStatus GetdeliveryStatus(string deliveryStatus)
        {
            return (DeliveryStatus)Enum.Parse(typeof(DeliveryStatus), deliveryStatus, true);
        }
    }
}