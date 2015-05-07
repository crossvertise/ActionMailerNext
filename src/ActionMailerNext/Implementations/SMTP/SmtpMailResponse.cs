using System;

namespace ActionMailerNext.Implementations.SMTP
{
    using System.Net.Mail;
    using ActionMailerNext.Interfaces;

    public class SmtpMailResponse : IMailResponse
    {
        public string Email { get; set; }

        public SmtpStatusCode Status { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        public string RejectReason { get; set; }

        public override string ToString()
        {
            return String.Format("Email : {0}\nStatus : {1}\nRejection Reason : {2}", Email, Status, RejectReason);
        }

        public static SmtpStatusCode GetProspectiveStatus(string statusString)
        {
            return (SmtpStatusCode)Enum.Parse(typeof(SmtpStatusCode), statusString, true);
        }

        public static DeliveryStatus GetdeliveryStatus(string deliveryStatus)
        {
            return (DeliveryStatus)Enum.Parse(typeof(DeliveryStatus), deliveryStatus, true);
        }
    }

}
