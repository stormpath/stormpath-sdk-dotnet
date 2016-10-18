using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    public interface IPhoneCollection : IAsyncQueryable<IPhone>
    {
        Task<IPhone> AddPhoneAsync(string number);

        // TODO
        //Task<IPhone> AddPhoneAsync(PhoneCreationOptions options);
    }
}
