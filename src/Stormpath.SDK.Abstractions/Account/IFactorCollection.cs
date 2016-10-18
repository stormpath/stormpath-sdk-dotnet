using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    public interface IFactorCollection : IAsyncQueryable<IFactor>
    {
        Task<ISmsFactor> AddAsync(
            SmsFactorCreationOptions options,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IGoogleAuthenticatorFactor> AddAsync(
            GoogleAuthenticatorFactorCreationOptions options,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
