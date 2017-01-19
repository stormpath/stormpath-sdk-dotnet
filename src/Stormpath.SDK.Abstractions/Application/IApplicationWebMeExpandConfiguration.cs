namespace Stormpath.SDK.Application
{
    public interface IApplicationWebMeExpandConfiguration
    {
        bool? ApiKeys { get; }

        bool? Applications { get; }

        bool? CustomData { get; }

        bool? Directory { get; }

        bool? GroupMemberships { get; }

        bool? Groups { get; }

        bool? ProviderData { get; }

        bool? Tenant { get; }
    }
}
