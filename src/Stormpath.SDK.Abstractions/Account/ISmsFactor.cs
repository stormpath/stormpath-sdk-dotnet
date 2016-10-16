using System.Threading.Tasks;

namespace Stormpath.SDK.Account
{
    public interface ISmsFactor : IFactor
    {
        // TODO properties

        Task<IPhone> GetPhoneAsync();
    }
}
