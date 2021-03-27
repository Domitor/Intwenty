using Intwenty.DataClient;
using Intwenty.Entity;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    public interface IIntwentyDbLoggerService
    {
       
        Task LogErrorAsync(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        Task LogWarningAsync(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        Task LogInfoAsync(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        Task LogIdentityActivityAsync(string verbosity, string message, string username = "");

        Task<List<EventLog>> GetEventLogAsync(string verbosity, string logname);
    }

}
