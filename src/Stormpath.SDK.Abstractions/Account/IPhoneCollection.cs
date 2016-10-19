using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    public interface IPhoneCollection : IAsyncQueryable<IPhone>
    {
        Task<IPhone> AddAsync(string number, CancellationToken cancellationToken = default(CancellationToken));

        Task<IPhone> AddAsync(PhoneCreationOptions options, CancellationToken cancellationToken = default(CancellationToken));
    }
}
