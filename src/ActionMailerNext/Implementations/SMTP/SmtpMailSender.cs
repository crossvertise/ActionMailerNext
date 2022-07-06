namespace ActionMailerNext.Implementations.SMTP
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;

    using Interfaces;

    public class SmtpMailSender : IMailSender
    {
        private readonly IMailInterceptor _interceptor;
        private readonly SmtpClient _client;

        public SmtpMailSender()
            : this(new SmtpClient(), null)
        {
        }

        public SmtpMailSender(SmtpClient client, IMailInterceptor interceptor)
        {
            _interceptor = interceptor;
            _client = client;
        }

        protected MailMessage GenerateProspectiveMailMessage(MailAttributes mail)
        {
            var message = new MailMessage();

            for (int i = 0; i < mail.To.Count; i++)
            {
                message.To.Add(mail.To[i]);
            }

            for (int i = 0; i < mail.Cc.Count; i++)
            {
                message.CC.Add(mail.Cc[i]);
            }

            for (int i = 0; i < mail.Bcc.Count; i++)
            {
                message.Bcc.Add(mail.Bcc[i]);
            }

            for (int i = 0; i < mail.ReplyTo.Count; i++)
            {
                message.ReplyToList.Add(mail.ReplyTo[i]);
            }

            if (!string.IsNullOrWhiteSpace(mail.From.Address))
            {
                message.From = new MailAddress(mail.From.Address, mail.From.DisplayName);
            }

            message.Subject = mail.Subject;
            // https://connect.microsoft.com/VisualStudio/feedback/details/785710/mailmessage-subject-incorrectly-encoded-in-utf-8-base64
            message.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
            message.BodyEncoding = Encoding.UTF8;
            message.Priority = mail.Priority;

            foreach (var item in mail.Headers)
            {
                message.Headers[item.Key] = item.Value;
            }

            foreach (var item in mail.Attachments)
            {
                message.Attachments.Add(Utils.AttachmentCollection.ModifyAttachmentProperties(item.Key, item.Value, false));
            }

            foreach (var item in mail.Attachments.Inline)
            {
                message.Attachments.Add(Utils.AttachmentCollection.ModifyAttachmentProperties(item.Key, item.Value, true));
            }

            foreach (var view in mail.AlternateViews)
            {
                message.AlternateViews.Add(view);
            }

            return message;
        }

        public virtual List<IMailResponse> Deliver(IEmailResult emailResult) => Send(emailResult.MailAttributes);

        /// <summary>
        /// Sends SMTPMailMessage synchronously.
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
        /// Sends your message asynchronously.  This method does not block.  If you need to know
        /// when the message has been sent, then override the OnMailSent method in MailerBase which
        /// will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public async Task<MailAttributes> DeliverAsync(IEmailResult emailResult)
        {
            var deliverTask = SendAsync(emailResult.MailAttributes);
            await deliverTask.ContinueWith(t => AsyncSendCompleted(emailResult));

            return emailResult.MailAttributes;
        }

        /// <summary>
        /// Sends SMTPMailMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The MailAttributes message you wish to send.</param>
        /// <returns></returns>
        public virtual async Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes)
        {
            var response = new List<IMailResponse>();

            var mail = GenerateProspectiveMailMessage(mailAttributes);
            try
            {
                await _client.SendMailAsync(mail);
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

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing) { }

        private void AsyncSendCompleted(IEmailResult email) => _interceptor.OnMailSent(email.MailAttributes);
    }
}