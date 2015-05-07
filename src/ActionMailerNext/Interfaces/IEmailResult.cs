using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActionMailerNext.Interfaces
{
    /// <summary>
    ///     Interface for email results. Allows to send the message synchonously or asynchonously
    /// </summary>
    public interface IEmailResult
    {
        /// <summary>
        ///     The underlying MailAttributes object that was passed to this object's constructor.
        /// </summary>
        MailAttributes MailAttributes { get; }

        /// <summary>
        ///     The default encoding used to send a message.
        /// </summary>
        Encoding MessageEncoding { get; }
    }
}