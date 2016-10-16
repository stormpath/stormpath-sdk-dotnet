using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    public interface IFactorCollection : IAsyncQueryable<IFactor>
    {
        Task<ISmsFactor> AddAsync(SmsFactorCreationOptions options);

        // TODO ITotpFactor instead?
        Task<IGoogleAuthenticatorFactor> AddAsync(GoogleAuthenticatorFactorCreationOptions options);
    }
}
