namespace Stormpath.SDK.Application
{
    public sealed class ClientApiWebMeConfiguration
    {
        public bool? Enabled { get; set; }

        public ClientApiWebMeExpandConfiguration Expand { get; set; }
    }
}
