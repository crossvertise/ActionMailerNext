namespace ActionMailerNext.Standalone
{
    /// <summary>
    ///     Enum to choose which method the email should be sent with.
    /// </summary>
    public enum MailMethod
    {
        /// <summary>
        ///     SMTP Method
        /// </summary>
        SMTP,

        /// <summary>
        ///     Mandrill HTTP Method
        /// </summary>
        Mandrill
    }
}