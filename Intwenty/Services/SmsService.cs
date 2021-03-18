using Intwenty.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intwenty.Model;

namespace Intwenty.Services
{
    
    public class SmsService : IIntwentySmsService
    {

        public IntwentySettings Settings { get; }

        public SmsService(IOptions<IntwentySettings> settings)
        {
            Settings = settings.Value;
        }


        public virtual Task SendSmsAsync(string number, string message)
        {
            return Task.FromResult(0);
        }
    }

}