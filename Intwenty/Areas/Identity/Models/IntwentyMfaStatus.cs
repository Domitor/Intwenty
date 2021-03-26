using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{

    public class IntwentyMfaStatus
    {

        public IntwentyMfaStatus()
        {
        }

        public bool HasAnyMFA 
        {
            get
            {
                if (HasBankIdMFA || HasSmsMFA || HasEmailMFA || HasFido2MFA || HasTotpMFA || HasFrejaMFA)
                    return true;

                return false;
            }
        
        }

        public bool HasBankIdMFA { get; set; }
        public bool HasSmsMFA { get; set; }
        public bool HasEmailMFA { get; set; }
        public bool HasFido2MFA { get; set; }
        public bool HasTotpMFA { get; set; }
        public bool HasFrejaMFA { get; set; }


    }
}
