using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultAdLdapProvider : AbstractProvider, IAdLdapProvider
    {
        public DefaultAdLdapProvider(ResourceData data)
            : base(data)
        {
        }
    }
}
