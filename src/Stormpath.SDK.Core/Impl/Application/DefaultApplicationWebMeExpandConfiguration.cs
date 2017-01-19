using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed class DefaultApplicationWebMeExpandConfiguration : IApplicationWebMeExpandConfiguration
    {
        public bool? ApiKeys { get; set; }
        public bool? Applications { get; set; }
        public bool? CustomData { get; set; }
        public bool? Directory { get; set; }
        public bool? GroupMemberships { get; set; }
        public bool? Groups { get; set; }
        public bool? ProviderData { get; set; }
        public bool? Tenant { get; set; }
    }
}
