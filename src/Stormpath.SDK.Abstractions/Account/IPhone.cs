using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    public interface IPhone : 
        IResource,
        IDeletable,
        ISaveable<IPhone>,
        IAuditable
    {
        string Number { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        PhoneVerificationStatus VerificationStatus { get; }

        PhoneStatus Status { get; set; }

        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
