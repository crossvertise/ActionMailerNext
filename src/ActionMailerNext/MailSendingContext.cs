using ActionMailerNext.Interfaces;

namespace ActionMailerNext
{
    /// <summary>
    ///     A special context object used by the OnMailSending() method
    ///     to allow you to inspect the underlying MailMessage before it
    ///     is sent, or prevent it from being sent altogether.
    /// </summary>
    public class MailSendingContext
    {
        /// <summary>
        ///     The generated mail message that is being sent.
        /// </summary>
        public readonly MailAttributes Mail;

        /// <summary>
        ///     A special flag that you can toggle to prevent this mail
        ///     from being sent.
        /// </summary>
        public bool Cancel;

        /// <summary>
        ///     Returns a populated context to be used for the OnMailSending()
        ///     method in MailerBase.
        /// </summary>
        /// <param name="mail">The message you wish to wrap within this context.</param>
        public MailSendingContext(MailAttributes mail)
        {
            Mail = mail;
            Cancel = false;
        }
    }
}