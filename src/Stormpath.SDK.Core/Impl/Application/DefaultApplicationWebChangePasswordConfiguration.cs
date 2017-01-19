using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationWebChangePasswordConfiguration : IApplicationWebChangePasswordConfiguration
    {
        public bool? Enabled { get; set; }
    }
}
