using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationWebRegisterConfiguration : IApplicationWebRegisterConfiguration
    {
        public bool? Enabled { get; set; }
    }
}
