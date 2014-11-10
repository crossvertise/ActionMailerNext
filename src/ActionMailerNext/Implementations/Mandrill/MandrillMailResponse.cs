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
        public string Status { get; set; }
        public string RejectReason { get; set; }

        public string Id { get; set; }
    }
}
