namespace ActionMailerNext.Utils
{
    using System.Collections.Generic;
    using System.Net.Mail;

    public class AlternativeViewCollection : Dictionary<string, AlternateView>
    {
        public AlternativeViewCollection()
        {
            Inline = new Dictionary<string, AlternateView>();
        }

        /// <summary>
        /// Any attachments added to this collection will be treated as inline attachments within the mail message.
        /// </summary>
        public Dictionary<string, AlternateView> Inline { get; private set; }
    }
}