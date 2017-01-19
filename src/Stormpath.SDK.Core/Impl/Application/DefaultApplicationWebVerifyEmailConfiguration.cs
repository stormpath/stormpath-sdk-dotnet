using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationWebVerifyEmailConfiguration : IApplicationWebVerifyEmailConfiguration
    {
        public bool? Enabled { get; set; }
    }
}
