namespace Stormpath.SDK.Application
{
    public interface IApplicationWebMeConfiguration
    {
        bool? Enabled { get; }

        IApplicationWebMeExpandConfiguration Expand { get; }
    }
}
