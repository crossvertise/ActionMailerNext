using ActionMailer.Net.Implementations.Mandrill;
using ActionMailer.Net.Implementations.SMTP;
using ActionMailer.Net.Interfaces;

namespace ActionMailer.Net.Standalone
{
    internal static class MailMethodUtil
    {
        
        public static IMailSender GetSender(MailMethod method = MailMethod.SMTP)
        {
            switch (method)
            {
                case MailMethod.SMTP :
                {
                    return new SmtpMailSender();
                }
                case MailMethod.Mandrill :
                {
                    return new MandrillMailSender();
                }
                default:
                {
                    return null;
                }
            }
        }

        public static IMailAttributes GetAttributes(MailMethod method = MailMethod.SMTP)
        {
            switch (method)
            {
                case MailMethod.SMTP:
                    {
                        return new SmtpMailAttributes();
                    }
                case MailMethod.Mandrill:
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

    /// <summary>
    /// Enum to choose which method the email should be sent with.
    /// </summary>
    public enum MailMethod
    {
        /// <summary>
        /// SMTP Method
        /// </summary>
        SMTP,
        /// <summary>
        /// Mandrill HTTP Method
        /// </summary>
        Mandrill
    }


   
}
