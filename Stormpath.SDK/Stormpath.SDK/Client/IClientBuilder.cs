namespace Stormpath.SDK.Client
{
    public interface IClientBuilder
    {
        IClientBuilder SetApiKey(ApiKey apiKey);

        IClientBuilder SetAuthenticationScheme(AuthenticationScheme authenticationScheme);

        IClientBuilder SetConnectionTimeout(int timeout);

        IClientBuilder SetBaseUrl(string baseUrl);

        IClient Build();
    }
}
