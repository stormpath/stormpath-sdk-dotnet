using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationWebOauth2Configuration : IApplicationWebOauth2Configuration
    {
        public bool? Enabled { get; set; }
    }
}
