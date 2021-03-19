using Intwenty.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intwenty.Model;

namespace IntwentyDemo.Services
{
    
    public class AspSmsService : IIntwentySmsService
    {

        public IntwentySettings Settings { get; }

        public AspSmsService(IOptions<IntwentySettings> settings)
        {
            Settings = settings.Value;
        }


        public virtual Task SendSmsAsync(string number, string message)
        {
            ASPSMS.SMS SMSSender = new ASPSMS.SMS();

            SMSSender.Userkey = Settings.SmsServiceUser;
            SMSSender.Password = Settings.SmsServicePwd;
            SMSSender.Originator = Settings.SmsServiceFrom;

            SMSSender.AddRecipient(number);

            SMSSender.MessageData = message;

            SMSSender.SendTextSMS();

            return Task.FromResult(0);
        }
    }

}