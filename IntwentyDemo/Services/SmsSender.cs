using Intwenty.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntwentyDemo.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }

    public class SmsSender : ISmsSender
    {
        public IntwentySettings Settings { get; }

        public SmsSender(IOptions<IntwentySettings> settings)
        {
            Settings = settings.Value;
        }

      

        public Task SendSmsAsync(string number, string message)
        {
            return Task.CompletedTask;
        }
    }

}