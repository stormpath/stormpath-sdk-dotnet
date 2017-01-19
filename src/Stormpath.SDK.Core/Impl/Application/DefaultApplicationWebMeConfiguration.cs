using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationWebMeConfiguration : IApplicationWebMeConfiguration
    {
        public bool? Enabled { get; set; }

        public IApplicationWebMeExpandConfiguration Expand { get; set; }
    }
}
