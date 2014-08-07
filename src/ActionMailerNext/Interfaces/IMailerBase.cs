using ActionMailer.Net.Utils;

namespace ActionMailer.Net.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMailerBase : IMailInterceptor
    {
        /// <summary>
        /// The underlying IMailSender to use for outgoing messages.
        /// </summary>
        IMailSender MailSender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        void SetMailMethod(MailMethod method);

    }
}