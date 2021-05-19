using Intwenty.Model.BankId;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    public interface IBankIDService
    {
        Task<AuthResponse> Auth(AuthRequest authRequest);
        Task<AuthResponse> Sign(SignRequest signRequest);
        Task<CollectResponse> Collect(CollectRequest collectRequest);
        Task<bool> Cancel(CancelRequest signRequest);

    }
}