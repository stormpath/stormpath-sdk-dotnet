using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationWebForgotPasswordConfiguration : IApplicationWebForgotPasswordConfiguration
    {
        public bool? Enabled { get; set; }
    }
}
