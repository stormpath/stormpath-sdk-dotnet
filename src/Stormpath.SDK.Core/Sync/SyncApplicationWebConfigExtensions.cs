using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Application;

namespace Stormpath.SDK.Sync
{
    public static class SyncApplicationWebConfigExtensions
    {
        public static IApiKey GetSigningApiKey(this IApplicationWebConfiguration appWebConfig)
            => (appWebConfig as IApplicationWebConfigurationSync).GetSigningApiKey();

        public static IApplication GetApplication(this IApplicationWebConfiguration appWebConfig)
            => (appWebConfig as IApplicationWebConfigurationSync).GetApplication();
    }
}
