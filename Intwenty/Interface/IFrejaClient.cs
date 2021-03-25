using Intwenty.DataClient;
using Intwenty.Entity;
using Intwenty.Model;
using Intwenty.Model.FrejaEId;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    public interface IFrejaClient
    {
        Task<FrejaStatusResponse> InitQRAuthentication();
        Uri GetQRCode(string authref);
        Task<bool> Authenticate(string authref);
        Task<bool> Authenticate(string userInfoType, string userInfo);
        Task<bool> Sign(string userInfoType, string userInfo, Signature signature);
        SignatureValidationResult Validate(Signature signature);
    }

}
