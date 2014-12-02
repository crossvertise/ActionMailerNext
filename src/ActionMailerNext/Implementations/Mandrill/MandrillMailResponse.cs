using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionMailerNext.Interfaces;

namespace ActionMailerNext.Implementations.Mandrill
{
    public class MandrillMailResponse : IMailResponse
    {
        public string Email { get; set; }

        public MandrillStatus Status { get; set; }

        public string RejectReason { get; set; }

        public string Id { get; set; }

        public override string ToString()
        {
            return String.Format("Id : {0}\nEmail : {1}\nStatus : {2}\nRejection Reason : {3}", Id, Email, Status,
                RejectReason);
        }

        public static MandrillStatus GetProspectiveStatus(string statusString)
        {
            return (MandrillStatus)Enum.Parse(typeof(MandrillStatus), statusString, true);
        }
    }
    
}
