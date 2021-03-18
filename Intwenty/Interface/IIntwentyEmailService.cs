using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Intwenty.Interface
{
    public interface IIntwentyEmailService
    {
        Task SendEmailAsync(string sendto, string subject, string message);
    }

}
