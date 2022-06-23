﻿namespace ActionMailerNext.Implementations.SMTP
{
    using System;
    using System.Net.Mail;

    using ActionMailerNext.Interfaces;

    public class SmtpMailResponse : IMailResponse
    {
        public string Email { get; set; }

        public SmtpStatusCode Status { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        public string RejectReason { get; set; }

        public override string ToString() => string.Format("Email : {0}\nStatus : {1}\nRejection Reason : {2}", Email, Status, RejectReason);

        public static SmtpStatusCode GetProspectiveStatus(string statusString) => (SmtpStatusCode)Enum.Parse(typeof(SmtpStatusCode), statusString, true);

        public static DeliveryStatus GetdeliveryStatus(string deliveryStatus) => (DeliveryStatus)Enum.Parse(typeof(DeliveryStatus), deliveryStatus, true);
    }
}
