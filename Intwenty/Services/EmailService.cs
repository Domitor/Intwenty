using Intwenty.Interface;
using Intwenty.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;



namespace Intwenty.Services
{
   
    public class EmailService : IIntwentyEmailService
    {
        private IntwentySettings Settings { get; }

        public EmailService(IOptions<IntwentySettings> settings)
        {
            Settings = settings.Value;
        }


        public virtual async Task SendEmailAsync(string sendto, string subject, string message)
        {
            if (Settings.RedirectAllOutgoingMailTo.Contains("@") && Settings.RedirectAllOutgoingMailTo.Contains("."))
                await Send(Settings.RedirectAllOutgoingMailTo, subject, message);
            else
                await Send(sendto, subject, message);

        }

        protected virtual async Task Send(string sendto, string subject, string message)
        {

            if (string.IsNullOrEmpty(Settings.MailServiceServer) || string.IsNullOrEmpty(Settings.MailServiceUser) || string.IsNullOrEmpty(Settings.MailServicePwd))
                return;

                var mail = new MailMessage();

                mail.To.Add(sendto);

                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(Settings.MailServiceFromEmail);

                var client = new SmtpClient();
                client.Host = Settings.MailServiceServer;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(Settings.MailServiceUser, Settings.MailServicePwd);
                client.Port = Settings.MailServicePort;

                await client.SendMailAsync(mail);
          
        }

       
    }

}
