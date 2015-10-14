﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActionMailerNext.Implementations.SMTP
{
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Interfaces;

    /// <summary>
    ///     Implements IMailSender by using System.Net.MailAttributes.SmtpClient.
    /// </summary>
    public class SmtpMailSender : IMailSender
    {
        private readonly SmtpClient _client;

        /// <summary>
        ///     Creates a new SMTPMailMessage sender based on System.Net.MailAttributes.SmtpClient
        /// </summary>
        public SmtpMailSender() : this(new SmtpClient())
        {
        }

        /// <summary>
        ///     Creates a new SMTPMailMessage sender based on System.Net.MailAttributes.SmtpClient
        /// </summary>
        /// <param name="client">The underlying SmtpClient instance to use.</param>
        public SmtpMailSender(SmtpClient client)
        {
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
            if (!String.IsNullOrWhiteSpace(mail.From.Address))
                message.From = new MailAddress(mail.From.Address, mail.From.DisplayName);


            message.Subject = mail.Subject;
            message.SubjectEncoding = mail.SubjectEncoding == null ? Encoding.GetEncoding("ISO-8859-1") : mail.SubjectEncoding; //https://connect.microsoft.com/VisualStudio/feedback/details/785710/mailmessage-subject-incorrectly-encoded-in-utf-8-base64
            message.BodyEncoding = mail.MessageEncoding;
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

        /// <summary>
        ///     Sends SMTPMailMessage synchronously.
        /// </summary>
        /// <param name="mailAttributes">The MailAttributes you wish to send.</param>
        public virtual List<IMailResponse> Send(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            _client.Send(mail);

            return null;
        }

        /// <summary>
        ///     Sends SMTPMailMessage asynchronously using tasks.
        /// </summary>
        /// <param name="mailAttributes">The MailAttributes message you wish to send.</param>
        public virtual async Task<List<IMailResponse>> SendAsync(MailAttributes mailAttributes)
        {
            var mail = GenerateProspectiveMailMessage(mailAttributes);
            _client.SendMailAsync(mail);

            return null;
        }

        /// <summary>
        ///     Destroys the underlying SmtpClient.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
