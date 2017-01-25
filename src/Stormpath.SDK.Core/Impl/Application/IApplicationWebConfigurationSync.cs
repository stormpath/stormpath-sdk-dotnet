using Stormpath.SDK.Api;
using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal interface IApplicationWebConfigurationSync
    {
        IApiKey GetSigningApiKey();

        IApplication GetApplication();
    }
}
