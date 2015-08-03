namespace Stormpath.SDK.Api
{
    public sealed class ClientApiKeys
    {
        public static IClientApiKeyBuilder Builder()
        {
            return new Impl.Api.DefaultClientApiKeyBuilder(
                new Impl.Utility.ConfigurationManagerWrapper(), 
                new Impl.Utility.EnvironmentWrapper(),
                new Impl.Utility.FileWrapper());
        }
    }
}
