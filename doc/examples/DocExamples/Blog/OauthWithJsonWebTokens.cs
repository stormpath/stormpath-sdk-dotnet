using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.DocExamples.Blogs
{
    /// <summary>
    /// Code listings for https://stormpath.com/blog/token-based-authentication-in-dot-net
    /// </summary>
    public class OauthWithJsonWebTokens
    {
        private async Task GettingOauthPolicy()
        {
            IApplication myApp = null;

            #region code
            var policy = await myApp.GetOauthPolicyAsync();
            // Default values:
            //policy.AccessTokenTimeToLive = TimeSpan.FromHours(1);
            //policy.RefreshTokenTimeToLive = TimeSpan.FromDays(60);
            #endregion

            Console.WriteLine(policy.AccessTokenTimeToLive);
            Console.WriteLine(policy.RefreshTokenTimeToLive);
        }

        private async Task SettingOauthPolicy()
        {
            IOauthPolicy policy = null;

            #region code
            policy.SetAccessTokenTimeToLive(TimeSpan.FromDays(1));
            policy.SetRefreshTokenTimeToLive(TimeSpan.FromDays(180));
            await policy.SaveAsync();
            #endregion
        }

        private async Task PasswordGrantRequest()
        {
            IApplication myApp = null;

            #region code
            // Build the Password Grant request
            var passwordGrantRequest = OauthRequests.NewPasswordGrantRequest()
                .SetLogin("lskywalker@tattooine.rim")
                .SetPassword("whataPieceofjunk$1138")
                .Build();

            // Send the request to Stormpath
            var grantResult = await myApp.NewPasswordGrantAuthenticator()
                .AuthenticateAsync(passwordGrantRequest);

            // grantResult.AccessTokenHref is the location of the created Access Token resource in Stormpath
            // grantResult.ExpiresIn is the number of seconds the access token is valid
            // grantResult.AccessTokenString is the access token JWT
            // grantResult.RefreshTokenString is the refresh token JWT
            // grantResult.GetAccessTokenAsync() will retrieve the resource as an IAccessToken
            #endregion

            Console.WriteLine(grantResult.AccessTokenHref);
            Console.WriteLine(grantResult.ExpiresIn);
            Console.WriteLine(grantResult.AccessTokenString);
            Console.WriteLine(grantResult.RefreshTokenString);
            var token = await grantResult.GetAccessTokenAsync();
        }

        private async Task RemoteValidation()
        {
            string accessTokenJwtString = null;
            IApplication myApp = null;

            #region code
            // Build the validation request
            var jwtAuthenticationRequest = OauthRequests.NewJwtAuthenticationRequest()
                .SetJwt(accessTokenJwtString)
                .Build();

            // If the request is successful, an IAccessToken is returned.
            // If the token is invalid, expired, revoked, or tampered with,
            // a ResourceException is thrown.
            IAccessToken validAccessToken = await myApp.NewJwtAuthenticator()
                .AuthenticateAsync(jwtAuthenticationRequest);
            #endregion
        }

        private async Task LocalValidation()
        {
            IApplication myApp = null;
            IJwtAuthenticationRequest jwtAuthenticationRequest = null;

            #region code
            // If the request is successful, an IAccessToken is returned.
            // If the token is invalid, expired, revoked, or tampered with,
            // the appropriate exception derived from InvalidJwtException is thrown.
            IAccessToken validAccessToken = await myApp.NewJwtAuthenticator()
                .WithLocalValidation()
                .AuthenticateAsync(jwtAuthenticationRequest);
            #endregion
        }

        private async Task RefreshGrantRequest()
        {
            string refreshTokenJwtString = null;
            IApplication myApp = null;

            #region code
            // Build the Refresh Grant request
            var refreshGrantRequest = OauthRequests.NewRefreshGrantRequest()
                .SetRefreshToken(refreshTokenJwtString) // the refresh token JWT
                .Build();

            // Send the request to Stormpath
            var refreshGrantResult = await myApp.NewRefreshGrantAuthenticator()
                .AuthenticateAsync(refreshGrantRequest);

            // refreshGrantResult.AccessTokenHref is the location of the created Access Token resource in Stormpath
            // refreshGrantResult.ExpiresIn is the number of seconds the access token is valid
            // refreshGrantResult.AccessTokenString is the new access token JWT
            // refreshGrantResult.RefreshTokenString is the refresh token JWT
            // refreshGrantResult.GetAccessTokenAsync() will retrieve the resource as an IAccessToken
            #endregion

            Console.WriteLine(refreshGrantResult.AccessTokenHref);
            Console.WriteLine(refreshGrantResult.ExpiresIn);
            Console.WriteLine(refreshGrantResult.AccessTokenString);
            Console.WriteLine(refreshGrantResult.RefreshTokenString);
            var token = await refreshGrantResult.GetAccessTokenAsync();
        }

        private async Task GetAllTokens()
        {
            IAccount account = null;

            #region code
            var accessTokens = await account.GetAccessTokens().ToListAsync();
            var refreshTokens = await account.GetRefreshTokens().ToListAsync();
            #endregion
        }

        private async Task GetTokenForApplication()
        {
            IAccount account = null;
            IApplication myApp = null;

            #region code
            var accessTokenForApplication = await account
                .GetAccessTokens()
                .Where(x => x.ApplicationHref == myApp.Href)
                .SingleOrDefaultAsync();
            #endregion
        }

        private async Task RevokeToken()
        {
            IAccessToken token = null;

            #region code
            // Get a reference to an IAccessToken or IRefreshToken by looking it up by href,
            // or by querying the GetAccessTokens() or GetRefreshTokens() collections.

            await token.DeleteAsync();
            #endregion
        }

        private void CreateJwt()
        {
            IClient client = null;

            #region code
            var builder = client.NewJwtBuilder();

            // IJwtBuilder supports setting any standard JWT claim,
            // plus arbitrary claims that you define:
            builder
                // Set the Audience (aud) claim
                .SetAudience("Darth Vader")
                // Set the Expiration (exp) claim
                .SetExpiration(DateTimeOffset.Now.AddDays(10))
                // Set the JWT ID (jti) claim
                .SetId($"jwt-id-{Guid.NewGuid()}")
                // Set the Issued-At (iat) claim
                .SetIssuedAt(DateTimeOffset.Now)
                // Set the Issuer (iss) claim
                .SetIssuer("Lord Sidious")
                // Set the Subject (sub) claim:
                .SetSubject("Secret Plans")
                // SetClaim() can be used to add any claim as a key-value pair:
                .SetClaim("title", "Death Star")
                // SignWith() is used to sign the JWT with a secret key:
                .SignWith("my_secret_key_123", Encoding.UTF8);

            // Build the JWT
            var jwt = builder.Build();
            #endregion
        }

        private void InspectJwt()
        {
            IJwt jwt = null;

            #region code
            // jwt.Base64Header, jwt.Base64Payload, and jwt.Base64Digest
            // contain the original base64 parts of the JWT
            string base64Digest = jwt.Base64Digest;

            // The header is represented as a dictionary
            var alg = jwt.Header["alg"];

            // The deserialized body is available via Body
            string aud = jwt.Body.Audience;
            DateTimeOffset exp = jwt.Body.Expiration.Value;

            // IJwtClaims includes some helper methods for working with claims
            bool containsTitle = jwt.Body.ContainsClaim("title"); // true
            string title = jwt.Body.GetClaim("title").ToString();

            // Get a string representation of the entire JWT
            string token = jwt.ToString();
            #endregion
        }

        private void ParseJwt()
        {
            IClient client = null;
            string token = null;

            #region code
            var parser = client.NewJwtParser();

            // Parse and validate a JWT
            var parsedJwt = parser
                // Sets the secret key used to sign the JWT
                .SetSigningKey("my_secret_key_123", Encoding.UTF8)
                // Verifies and deserializes the JWT
                .Parse(token);
            #endregion
        }

        private void ParseJwtCustomClaims()
        {
            IJwtParser parser = null;
            string token = null;

            #region code
            var parsedJwt = parser
                // If these claims do not exist, a MissingClaimException is thrown
                // If the claim exists but the value does not match, a MismatchedClaimException is thrown
                .RequireAudience("Darth Vader")
                .RequireIssuer("Lord Sidious")
                .RequireClaim("title", "Death Star")
                .SetSigningKey("my_secret_key_123", Encoding.UTF8)
                // Verifies and deserializes the JWT
                .Parse(token);
            #endregion
        }
    }
}
