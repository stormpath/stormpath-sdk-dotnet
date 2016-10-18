using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.Account
{
    public interface ISmsFactor : IFactor
    {
        Task<IPhone> GetPhoneAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
