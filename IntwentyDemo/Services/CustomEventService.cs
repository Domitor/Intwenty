using Intwenty.Interface;
using Intwenty.Model;
using Intwenty.SystemEvents;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace IntwentyDemo.Services
{
    public class CustomEventService : IntwentyEventService
    {
        private IntwentySettings Settings { get; }
        public CustomEventService(IIntwentyEmailService emailsender, IIntwentyDataService dataservice, IOptions<IntwentySettings> settings) : base(emailsender, dataservice)
        {
            Settings = settings.Value;
        }

        public override async Task NewUserCreated(NewUserCreatedData data)
        {
            await base.NewUserCreated(data);

            var mailmsg = string.Format("<br /><br /><b>Thank you for registering an account at {0}</b><br /><br />", Settings.SiteTitle);
            mailmsg += $"Please confirm that this was sent to your email by <a href='{HtmlEncoder.Default.Encode(data.ConfirmCallbackUrl)}'>clicking here</a>";
            await EmailService.SendEmailAsync(data.Email, "Thank you for creating an account.", mailmsg);
        }

        public override async Task EmailChanged(EmailChangedData data)
        {

            await base.EmailChanged(data);

            var mailmsg = string.Format("<br /><br /><b>You update your email at {0}</b><br /><br />", Settings.SiteTitle);
            mailmsg += $"Please confirm by <a href='{HtmlEncoder.Default.Encode(data.ConfirmCallbackUrl)}'>clicking here</a>";

            await EmailService.SendEmailAsync(data.Email, "Please confirm changed email.", mailmsg);
        }

    }
}
