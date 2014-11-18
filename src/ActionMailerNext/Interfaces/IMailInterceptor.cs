namespace ActionMailerNext.Interfaces
{
    /// <summary>
    ///     A simple interface that allows for reading or manipulating mail
    ///     messages before and after transfer.
    /// </summary>
    public interface IMailInterceptor
    {
        /// <summary>
        ///     This method is called before each mail is sent
        /// </summary>
        /// <param name="context">
        ///     A simple context containing the mail
        ///     and a boolean value that can be toggled to prevent this
        ///     mail from being sent.
        /// </param>
        void OnMailSending(MailSendingContext context);

        /// <summary>
        ///     This method is called after each mail is sent.
        /// </summary>
        /// <param name="mail">The mail that was sent.</param>
        void OnMailSent(MailAttributes mail);
    }
}