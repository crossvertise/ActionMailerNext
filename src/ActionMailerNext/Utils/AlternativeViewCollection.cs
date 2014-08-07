using System.Collections.Generic;
using System.Net.Mail;

namespace ActionMailerNext.Utils
{
    /// <summary>
    ///     A collection of attachments.  This is basically a glorified Dictionary.
    /// </summary>
    public class AlternativeViewCollection : Dictionary<string, AlternateView>
    {
        /// <summary>
        ///     Constructs an empty AlternativeViewCollection object.
        /// </summary>
        public AlternativeViewCollection()
        {
            Inline = new Dictionary<string, AlternateView>();
        }

        /// <summary>
        ///     Any attachments added to this collection will be treated
        ///     as inline attachments within the mail message.
        /// </summary>
        public Dictionary<string, AlternateView> Inline { get; private set; }
    }
}