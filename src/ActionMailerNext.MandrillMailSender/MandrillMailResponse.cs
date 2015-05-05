using ActionMailerNext.Implementations.SMTP;
using ActionMailerNext.Interfaces;
using System;

namespace ActionMailerNext.MandrillMailSender
{
    public class MandrillMailResponse : IMailResponse
    {
        public string Email { get; set; }

        public MandrillStatus Status { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        public string RejectReason { get; set; }

        public string Id { get; set; }

        public override string ToString()
        {
            return String.Format("Id : {0}\nEmail : {1}\nStatus : {2}\nRejection Reason : {3}", Id, Email, Status, RejectReason);
        }

        public static MandrillStatus GetProspectiveStatus(string statusString)
        {
            return (MandrillStatus)Enum.Parse(typeof(MandrillStatus), statusString, true);
        }
    }
    
}
