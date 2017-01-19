using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationWebLoginConfiguration : IApplicationWebLoginConfiguration
    {
        public bool? Enabled { get; set; }
    }
}
