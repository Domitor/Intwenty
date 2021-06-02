using Intwenty.Model.BankId;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    public interface IBankIDClientService
    {
        Task<BankIDAuthResponse> InitAuthentication(BankIDAuthRequest authRequest);
        Task<BankIDCollectResponse> Authenticate(BankIDCollectRequest collectRequest);
        Task<bool> Cancel(BankIDCancelRequest signRequest);
        Task<BankIDAuthResponse> Sign(BankIDSignRequest signRequest);
        string GetQRCode(string autoStartToken);
    }
}