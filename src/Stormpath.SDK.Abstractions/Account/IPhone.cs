using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    public interface IPhone : 
        IResource,
        IDeletable,
        IAuditable
    {
        string Number { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        PhoneVerificationStatus VerificationStatus { get; set; }

        Task<IAccount> GetAccountAsync();
    }
}
