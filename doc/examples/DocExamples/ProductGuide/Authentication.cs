using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Client;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Error;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Provider;

namespace examples.ProductGuide
{
    public class Authentication
    {
        IClient client = null;

        public async Task LoginAttempt()
        {
            #region code/csharp/authentication/login_attempt_req.cs
            try
            {
                var app = await client.GetApplicationAsync(
                    "https://api.stormpath.com/v1/applications/$YOUR_APPLICATION_ID");

                var loginResult = await app.AuthenticateAccountAsync(
                    "han@newrepublic.gov", "first2shoot!");

                var accountDetails = await loginResult.GetAccountAsync();
            }
            catch (ResourceException rex)
            {
                // Bad login credentials!
                Console.WriteLine("Error logging in. " + rex.Message);
            }
            #endregion
        }

        public async Task LoginAttemptExpanded()
        {
            IApplication app = null;

            #region code/csharp/authentication/login_attempt_req_expand_account.cs
            var loginRequest = new UsernamePasswordRequestBuilder()
                .SetUsernameOrEmail("han@newrepublic.gov")
                .SetPassword("first2shoot!")
                .Build();

            var loginResult = await app.AuthenticateAccountAsync(
                loginRequest,
                opt => opt.Expand(result => result.GetAccount()));
            #endregion
        }

        public async Task CustomDataAccountLink()
        {
            IAccount account = null;

            #region code/csharp/authentication/customdata_accountlink.cs
            var customData = await account.GetCustomDataAsync();

            customData.Put("https://api.stormpath.com/v1/accounts/3fLduLKlEx");
            await customData.SaveAsync();
            #endregion
        }

        public async Task GetAccountStoreMappings()
        {
            #region code/csharp/authentication/get_asm_req.cs
            var app = await client.GetApplicationAsync(
                "https://api.stormpath.com/v1/applications/5nan67mWrYrBmLGu7nGurh");

            var accountStoreMappings = await app
                .GetAccountStoreMappings()
                .ToListAsync();
            #endregion
        }

        public async Task CreateDefaultAccountStoreMapping()
        {
            #region code/csharp/authentication/create_asm.cs
            var application = await client.GetApplicationAsync(
                "https://api.stormpath.com/v1/applications/1gk4Dxzi6o4PbdleXaMPL");

            var directory = await client.GetDirectoryAsync(
                "https://api.stormpath.com/v1/directories/2SKhstu8PlaekcaEXampLE");

            var newMapping = client.Instantiate<IApplicationAccountStoreMapping>()
                .SetApplication(application)
                .SetAccountStore(directory)
                .SetListIndex(0)
                .SetDefaultAccountStore(true)
                .SetDefaultGroupStore(true);

            await application.CreateAccountStoreMappingAsync(newMapping);
            #endregion
        }

        public async Task UpdateExistingMappingListIndex()
        {
            IApplicationAccountStoreMapping existingMapping = null;

            #region code/csharp/authentication/change_login_priority.cs
            existingMapping.SetListIndex(0);
            await existingMapping.SaveAsync();
            #endregion
        }

        public async Task UpdateExistingMappingDefaultStore()
        {
            IApplicationAccountStoreMapping existingMapping = null;

            #region code/csharp/authentication/change_default_stores.cs
            existingMapping.SetDefaultAccountStore(true);
            existingMapping.SetDefaultGroupStore(true);
            await existingMapping.SaveAsync();
            #endregion
        }

        public async Task GetOauthPolicy()
        {
            IApplication app = null;

            #region code/csharp/authentication/oauth_policy.cs
            var oauthPolicy = await app.GetOauthPolicyAsync();
            #endregion
        }

        public async Task UpdateOauthPolicyTtls()
        {
            IOauthPolicy oauthPolicy = null;

            #region code/csharp/authentication/update_oauth_ttl_req.cs
            oauthPolicy.SetAccessTokenTimeToLive(TimeSpan.FromMinutes(30));
            oauthPolicy.SetRefreshTokenTimeToLive(TimeSpan.FromDays(7));
            await oauthPolicy.SaveAsync();
            #endregion
        }

        public async Task OauthPasswordGrantFlow()
        {
            IApplication app = null;

            #region code/csharp/authentication/generate_oauth_token_req.cs
            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("han@newrepublic.gov")
                .SetPassword("first2shoot!")
                .Build();

            var grantResponse = await app.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);
            #endregion
        }

        public async Task OauthValidateAccessToken()
        {
            IApplication app = null;
            string access_token = null;

            #region code/csharp/authentication/validate_oauth_token_sp_req.cs
            var validationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(access_token)
                .Build();

            var accessToken = await app.NewJwtAuthenticator()
                .AuthenticateAsync(validationRequest);
            #endregion
        }

        public async Task OauthValidateAccessTokenLocally()
        {
            IApplication app = null;
            string access_token = null;

            #region code/csharp/authentication/validate_oauth_token_local.cs
            var validationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(access_token)
                .Build();

            var accessToken = await app.NewJwtAuthenticator()
                .WithLocalValidation()
                .AuthenticateAsync(validationRequest);
            #endregion
        }

        public async Task OauthRefreshAccessToken()
        {
            IApplication app = null;
            string refresh_token = null;

            #region code/csharp/authentication/refresh_access_token_req.cs
            var refreshGrantRequest = OauthRequests.NewRefreshGrantRequest()
                .SetRefreshToken(refresh_token)
                .Build();

            var grantResponse = await app.NewRefreshGrantAuthenticator()
                .AuthenticateAsync(refreshGrantRequest);
            #endregion
        }

        public async Task GetTokensForAccount()
        {
            IAccount account = null;
            string access_token = null;
            string refresh_token = null;

            #region code/csharp/authentication/get_access_tokens.cs
            var allAccessTokens = await account.GetAccessTokens().ToListAsync();
            var allRefreshTokens = await account.GetRefreshTokens().ToListAsync();

            var accessTokenToDelete = allAccessTokens.Where(at => at.Jwt == access_token);
            var refreshTokenToDelete = allRefreshTokens.Where(rt => rt.Jwt == refresh_token);
            #endregion
        }

        public async Task GetTokensForAccountForApplication()
        {
            IAccount account = null;

            #region code/csharp/authentication/get_access_tokens_for_app.cs
            var allAccessTokens = await account.GetAccessTokens()
                .Where(at => at.ApplicationHref == "https://api.stormpath.com/v1/applications/1gk4Dxzi6o4PbdlBVa6tfR")
                .ToListAsync();
            #endregion
        }

        public async Task DeleteTokens()
        {
            IAccessToken accessTokenToDelete = null;
            IRefreshToken refreshTokenToDelete = null;

            #region code/csharp/authentication/delete_user_access_tokens_req.cs
            await accessTokenToDelete.DeleteAsync();
            await refreshTokenToDelete.DeleteAsync();
            #endregion
        }

        public async Task CreateGoogleDirectory()
        {
            #region code/csharp/authentication/create_directory_google.cs
            //# using Stormpath.SDK.Provider;

            var googleDirectory = client.Instantiate<IDirectory>();
            googleDirectory.SetName("My Google Directory");

            await client.CreateDirectoryAsync(googleDirectory,
                options => options.ForProvider(
                    client.Providers()
                        .Google()
                        .Builder()
                        .SetClientId("YOUR_GOOGLE_CLIENT_ID")
                        .SetClientSecret("YOUR_GOOGLE_CLIENT_SECRET")
                        .SetRedirectUri("YOUR_GOOGLE_REDIRECT_URI")
                        .Build()
                    ));
            #endregion
        }

        public async Task GetGoogleAccountWithCode()
        {
            IApplication app = null;
            string code = null;

            #region code/csharp/authentication/create_account_google_providerdata_code.cs
            var request = client.Providers()
                .Google()
                .Account()
                .SetCode(code)
                .Build();
            var result = await app.GetAccountAsync(request);
            #endregion
        }

        public async Task GetGoogleAccountWithAccessToken()
        {
            IApplication app = null;
            string access_token = null;

            #region code/csharp/authentication/create_account_google_providerdata_access_token.cs
            var request = client.Providers()
                .Google()
                .Account()
                .SetAccessToken(access_token)
                .Build();
            var result = await app.GetAccountAsync(request);
            #endregion
        }

        public async Task CreateFacebookDirectory()
        {
            #region code/csharp/authentication/create_directory_fb.cs
            //# using Stormpath.SDK.Provider;

            var facebookDirectory = client.Instantiate<IDirectory>();
            facebookDirectory.SetName("My Facebook Directory");

            await client.CreateDirectoryAsync(facebookDirectory,
                options => options.ForProvider(
                    client.Providers()
                        .Facebook()
                        .Builder()
                        .SetClientId("YOUR_FACEBOOK_APP_ID")
                        .SetClientSecret("YOUR_FACEBOOK_APP_SECRET")
                        .Build()
                    ));
            #endregion
        }

        public async Task GetFacebookAccountWithAccessToken()
        {
            IApplication app = null;
            string access_token = null;
            
            #region code/csharp/authentication/create_account_fb_providerdata_access_token.cs
            var request = client.Providers()
                .Facebook()
                .Account()
                .SetAccessToken(access_token)
                .Build();
            var result = await app.GetAccountAsync(request);
            #endregion
        }

        public async Task CreateGithubkDirectory()
        {
            #region code/csharp/authentication/create_directory_github.cs
            //# using Stormpath.SDK.Provider;

            var githubDirectory = client.Instantiate<IDirectory>();
            githubDirectory.SetName("My Github Directory");

            await client.CreateDirectoryAsync(githubDirectory,
                options => options.ForProvider(
                    client.Providers()
                        .Github()
                        .Builder()
                        .SetClientId("YOUR_GITHUB_CLIENT_ID")
                        .SetClientSecret("YOUR_GITHUB_CLIENT_SECRET")
                        .Build()
                    ));
            #endregion
        }

        public async Task GetGithubAccountWithAccessToken()
        {
            IApplication app = null;
            string access_token = null;

            #region code/csharp/authentication/create_account_github_providerdata_access_token.cs
            var request = client.Providers()
                .Github()
                .Account()
                .SetAccessToken(access_token)
                .Build();
            var result = await app.GetAccountAsync(request);
            #endregion
        }

        public async Task CreateLinkedInDirectory()
        {
            #region code/csharp/authentication/create_directory_linkedin.cs
            //# using Stormpath.SDK.Provider;

            var linkedInDirectory = client.Instantiate<IDirectory>();
            linkedInDirectory.SetName("My LinkedIn Directory");

            await client.CreateDirectoryAsync(linkedInDirectory,
                options => options.ForProvider(
                    client.Providers()
                        .LinkedIn()
                        .Builder()
                        .SetClientId("YOUR_LINKEDIN_APP_ID")
                        .SetClientSecret("YOUR_LINKEDIN_APP_SECRET")
                        .Build()
            #endregion
        }

        public async Task GetLinkedInAccountWithAccessToken()
        {
            IApplication app = null;
            string access_token = null;

            #region code/csharp/authentication/create_account_linkedin_providerdata_access_token.cs
            var request = client.Providers()
                .LinkedIn()
                .Account()
                .SetAccessToken(access_token)
                .Build();
            var result = await app.GetAccountAsync(request);
            #endregion
        }

        public async Task GetSamlPolicy()
        {
            IApplication app = null;

            #region code/csharp/authentication/saml_policy_example.cs
            var samlPolicy = await app.GetSamlPolicyAsync();

            var samlServiceProvider = await samlPolicy.GetSamlServiceProviderAsync();
            // samlServiceProvider.Href is the SAML Service Provider URL

            var ssoInitiationEndpoint = await samlServiceProvider.GetSsoInitiationEndpointAsync();
            // ssoInitiationEndpoint.Href is the SSO Initiation Endpoint
            #endregion
        }

        public async Task CreateApiKeyForAccount()
        {
            IAccount account = null;

            #region code/csharp/authentication/create_apikey_req.cs
            var newApiKey = await account.CreateApiKeyAsync();
            #endregion
        }

        public async Task DeleteApiKey()
        {
            IApplication app = null;
            string api_key_id = null;

            #region code/csharp/authentication/delete_apikey.cs
            var apiKey = await app.GetApiKeyAsync(api_key_id);
            await apiKey.DeleteAsync();
            #endregion
        }

        public async Task DisableApiKey()
        {
            IApplication app = null;
            string api_key_id = null;

            #region code/csharp/authentication/disable_apikey.cs
            var apiKey = await app.GetApiKeyAsync(api_key_id);
            apiKey.SetStatus(ApiKeyStatus.Disabled);
            await apiKey.SaveAsync();
            #endregion
        }

        public async Task AuthenticateBasicApiRequest()
        {
            IApplication app = null;
            string api_key_id = null;
            string api_key_secret = null;

            #region code/csharp/authentication/authenticate_basic_req.cs
            var apiKeyAuthRequest = new ApiKeyRequestBuilder()
                .SetId(api_key_id)
                .SetSecret(api_key_secret)
                .Build();

            var result = await app.AuthenticateAccountAsync(apiKeyAuthRequest);
            var account = await result.GetAccountAsync();
            #endregion
        }
    }
}
