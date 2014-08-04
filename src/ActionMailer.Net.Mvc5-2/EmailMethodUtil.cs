using ActionMailer.Net.Implementations.Mandrill;
using ActionMailer.Net.Implementations.SMTP;
using ActionMailer.Net.Interfaces;

namespace ActionMailer.Net.Mvc5_2
{
    internal static class EmailMethodUtil
    {
        
        public static IMailSender GetSender(MailSenderMethod method = MailSenderMethod.SMTP)
        {
            switch (method)
            {
                case MailSenderMethod.SMTP :
                {
                    return new SmtpMailSender();
                }
                case MailSenderMethod.Mandrill :
                {
                    return new MandrillMailSender();
                }
                default:
                {
                    return null;
                }
            }
        }

        public static IMailAttributes GetAttributes(MailSenderMethod method = MailSenderMethod.SMTP)
        {
            switch (method)
            {
                case MailSenderMethod.SMTP:
                    {
                        return new SmtpMailAttributes();
                    }
                case MailSenderMethod.Mandrill:
                    {
                        return new MandrillMailAttributes();
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }

    public enum MailSenderMethod
    {
        SMTP,
        Mandrill
    }


   
}
