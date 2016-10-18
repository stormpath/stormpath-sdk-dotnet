using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultSmsFactor : DefaultFactor, ISmsFactor
    {
        private const string PhonePropertyName = "phone";

        public DefaultSmsFactor(ResourceData data)
            : base(data)
        {
        }

        public Task<IPhone> GetPhoneAsync(CancellationToken cancellationToken = new CancellationToken())
            => GetInternalAsyncDataStore().GetResourceAsync<IPhone>(
                GetLinkProperty(PhonePropertyName).Href,
                cancellationToken);
    }
}
