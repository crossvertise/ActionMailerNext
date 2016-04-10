using System;
using System.Collections.Generic;
using System.Text;

namespace ActionMailerNext.Implementations.SMTP
{
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Interfaces;

    /// <summary>
    ///     Implements IMailSender by using System.Net.MailAttributes.SmtpClient.
    /// </summary>
    public class SmtpMailSender : IMailSender, IDisposable
    {
        private readonly IMailInterceptor _interceptor;
        private readonly SmtpClient _client;

        /// <summary>
        ///     Creates a new SMTPMailMessage sender based on System.Net.MailAttributes.SmtpClient
        /// </summary>
        public SmtpMailSender()
            : this(new SmtpClient(), null)
        {
        }

        /// <summary>
        ///     Creates a new SMTPMailMessage sender based on System.Net.MailAttributes.SmtpClient
        /// </summary>
        /// <param name="client">The underlying SmtpClient instance to use.</param>
        public SmtpMailSender(SmtpClient client, IMailInterceptor interceptor)
        {
            _interceptor = interceptor;
            _client = client;
        }

        /// <summary>
        ///     Creates a MailMessage for the current SmtpMailAttribute instance.
        /// </summary>
        protected MailMessage GenerateProspectiveMailMessage(MailAttributes mail)
        {
            var message = new MailMessage();

            for (var i = 0; i < mail.To.Count; i++)
                message.To.Add(mail.To[i]);

            for (var i = 0; i < mail.Cc.Count; i++)
                message.CC.Add(mail.Cc[i]);

            for (var i = 0; i < mail.Bcc.Count; i++)
                message.Bcc.Add(mail.Bcc[i]);

            for (var i = 0; i < mail.ReplyTo.Count; i++)
                message.ReplyToList.Add(mail.ReplyTo[i]);

            // From is optional because it could be set in <mailSettings>
            if (mail.From != null && !String.IsNullOrWhiteSpace(mail.From.Address))
                message.From = new MailAddress(mail.From.Address, mail.From.DisplayName);


            message.Subject = mail.Subject;
            message.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1"); //https://connect.microsoft.com/VisualStudio/feedback/details/785710/mailmessage-subject-incorrectly-encoded-in-utf-8-base64
            message.BodyEncoding = Encoding.UTF8;
            message.Priority = mail.Priority;

            foreach (var kvp in mail.Headers)
                message.Headers[kvp.Key] = kvp.Value;

            foreach (var kvp in mail.Attachments)
                message.Attachments.Add(Utils.AttachmentCollection.ModifyAttachmentProperties(kvp.Key, kvp.Value, false));

            foreach (var kvp in mail.Attachments.Inline)
                message.Attachments.Add(Utils.AttachmentCollection.ModifyAttachmentProperties(kvp.Key, kvp.Value, true));

            foreach (var view in mail.AlternateViews)
                message.AlternateViews.Add(view);

            return message;
        }

        public virtual List<IMailResponse> Deliver(IEmailResult emailResult)
        {
            return this.Send(emailResult.MailAttributes);
        }

        /// <summary>
        ///     Sends SMTPMailMessage synchronously.
        /// </summary>
        /// <param name="mailAttributes">The MailAttributes you wish to send.</param>
        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
            var response = new List<IMailResponse>();

            var mail = GenerateProspectiveMailMessage(mailAttributes);
            try
            {
                _client.Send(mail);
                response.AddRange(mail.To.Select(mailAddr => new SmtpMailResponse()
                {
                    Email = mailAddr.Address,
                    Status = SmtpMailResponse.GetProspectiveStatus(SmtpStatusCode.Ok.ToString()),
                    RejectReason = null
                }));
            }
            catch (SmtpFailedRecipientsException ex)
            {
                response.AddRange(ex.InnerExceptions.Select(e => new SmtpMailResponse
                {
                    Email = e.FailedRecipient,
                    Status = SmtpMailResponse.GetProspectiveStatus(e.StatusCode.ToString()),
                    RejectReason = e.Message
                }));
            }
            return response;
        }

        /// <summary>
        ///     Sends your message asynchronously.  This method does not block.  If you need to know
        ///     when the message has been sent, then override the OnMailSent method in MailerBase which
        ///     will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public async Task<MailAttributes> DeliverAsync(IEmailResult emailResult)
        {
            var deliverTask = this.SendAsync(emailResult.MailAttributes);
            await deliverTask.ContinueWith(t => AsyncSendCompleted(emailResult));

            return emailResult.MailAttributes;
        }

        /// <summary>
        ///     Sends SMTPMailMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The MailAttributes message you wish to send.</param>
        public virtual async Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes)
        {
            var response = new List<IMailResponse>();

            var mail = GenerateProspectiveMailMessage(mailAttributes);
            try
            {
                _client.SendMailAsync(mail);
                response.AddRange(mail.To.Select(mailAddr => new SmtpMailResponse()
                {
                    Email = mailAddr.Address, 
                    Status = SmtpMailResponse.GetProspectiveStatus(SmtpStatusCode.Ok.ToString()), 
                    RejectReason = null
                }));
            }
            catch (SmtpFailedRecipientsException ex)
            {
                response.AddRange(ex.InnerExceptions.Select(e => new SmtpMailResponse
                {
                    Email = e.FailedRecipient,
                    Status = SmtpMailResponse.GetProspectiveStatus(e.StatusCode.ToString()),
                    RejectReason = e.Message
                }));
            }
            return response;
        }

        /// <summary>
        ///     Destroys the underlying SmtpClient.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing) { }

        private void AsyncSendCompleted(IEmailResult email)
        {
           _interceptor.OnMailSent(email.MailAttributes);
        }
    }
}