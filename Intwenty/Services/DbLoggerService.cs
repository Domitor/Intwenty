﻿using Intwenty.DataClient;
using Intwenty.DataClient.Model;
using Intwenty.Entity;
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
   
    public class DbLoggerService : IIntwentyDbLoggerService
    {
        private IntwentySettings Settings { get; }

        public DbLoggerService(IOptions<IntwentySettings> settings)
        {
            Settings = settings.Value;
        }

        public IDataClient GetDataClient()
        {
            return new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
        }

        public IDataClient GetIAMDataClient()
        {
            return new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
        }

        public async Task LogErrorAsync(string message, int applicationid = 0, string appmetacode = "NONE", string username = "")
        {
            await LogEventAsync("ERROR", message, applicationid, appmetacode, username);
        }

        public async Task LogWarningAsync(string message, int applicationid = 0, string appmetacode = "NONE", string username = "")
        {
            await LogEventAsync("WARNING", message, applicationid, appmetacode, username);
        }

        public async Task LogInfoAsync(string message, int applicationid = 0, string appmetacode = "NONE", string username = "")
        {
            await LogEventAsync("INFO", message, applicationid, appmetacode, username);
        }

        public async Task LogIdentityActivityAsync(string verbosity, string message, string username = "")
        {
            if (Settings.LogVerbosity == LogVerbosityTypes.Error && (verbosity == "WARNING" || verbosity == "INFO"))
                return;
            if (Settings.LogVerbosity == LogVerbosityTypes.Warning && verbosity == "INFO")
                return;

            var client = GetIAMDataClient();

            await client.OpenAsync();

            try
            {

                var parameters = new List<IIntwentySqlParameter>();
                parameters.Add(new IntwentySqlParameter("@Verbosity", verbosity));
                parameters.Add(new IntwentySqlParameter("@Message", message));
                parameters.Add(new IntwentySqlParameter("@AppMetaCode", "NONE"));
                parameters.Add(new IntwentySqlParameter("@ApplicationId", 0));
                parameters.Add(new IntwentySqlParameter("@UserName", username));

                var getdatecmd = client.GetDbCommandMap().Find(p => p.Key == "GETDATE" && p.DbEngine == Settings.IAMConnectionDBMS);

                await client.RunCommandAsync("INSERT INTO sysdata_EventLog (EventDate, Verbosity, Message, AppMetaCode, ApplicationId,UserName) VALUES (" + getdatecmd.Command + ", @Verbosity, @Message, @AppMetaCode, @ApplicationId,@UserName)", parameters: parameters.ToArray());

            }
            catch { }
            finally
            {
                await client.CloseAsync();
            }
        }

        public virtual async Task<List<EventLog>> GetEventLogAsync(string verbosity, string logname)
        {
            IDataClient client = null;
            if (logname.ToUpper() == "IAM")
                client = GetIAMDataClient();
            else
                client = GetDataClient();

            await client.OpenAsync();

            try
            {
                if (string.IsNullOrEmpty(verbosity))
                {

                    var result = await client.GetEntitiesAsync<EventLog>("SELECT * FROM sysdata_EventLog ORDER BY Id DESC", false);
                    return result;
                }
                else
                {
                    var parameters = new List<IIntwentySqlParameter>();
                    parameters.Add(new IntwentySqlParameter("@Verbosity", verbosity));
                    var result = await client.GetEntitiesAsync<EventLog>("SELECT * FROM sysdata_EventLog WHERE Verbosity=@Verbosity ORDER BY Id DESC", false, parameters.ToArray());
                    return result;
                }
            }
            catch (Exception ex)
            {
                await LogErrorAsync("Error fetching eventlog - GetEventLog(): " + ex.Message);
            }
            finally
            {
                await client.CloseAsync();
            }

            return new List<EventLog>();
        }

        private async Task LogEventAsync(string verbosity, string message, int applicationid = 0, string appmetacode = "NONE", string username = "")
        {
            if (Settings.LogVerbosity == LogVerbosityTypes.Error && (verbosity == "WARNING" || verbosity == "INFO"))
                return;
            if (Settings.LogVerbosity == LogVerbosityTypes.Warning && verbosity == "INFO")
                return;

            var client = GetDataClient();
            await client.OpenAsync();

            try
            {

                var parameters = new List<IIntwentySqlParameter>();
                parameters.Add(new IntwentySqlParameter("@Verbosity", verbosity));
                parameters.Add(new IntwentySqlParameter("@Message", message));
                parameters.Add(new IntwentySqlParameter("@AppMetaCode", appmetacode));
                parameters.Add(new IntwentySqlParameter("@ApplicationId", applicationid));
                parameters.Add(new IntwentySqlParameter("@UserName", username));

                var getdatecmd = client.GetDbCommandMap().Find(p => p.Key == "GETDATE" && p.DbEngine == Settings.DefaultConnectionDBMS);

                await client.RunCommandAsync("INSERT INTO sysdata_EventLog (EventDate, Verbosity, Message, AppMetaCode, ApplicationId,UserName) VALUES (" + getdatecmd.Command + ", @Verbosity, @Message, @AppMetaCode, @ApplicationId,@UserName)", parameters: parameters.ToArray());

            }
            catch { }
            finally
            {
                await client.CloseAsync();
            }
        }
    }

}
