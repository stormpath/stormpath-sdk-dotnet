Imports Stormpath.SDK
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Api
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Auth
Imports Stormpath.SDK.Client
Imports Stormpath.SDK.Directory
Imports Stormpath.SDK.Error
Imports Stormpath.SDK.Oauth
Imports Stormpath.SDK.Provider

Public Class Authentication

    Private client As IClient = Nothing

    Public Async Function LoginAttempt() As Task
#Region "code/vbnet/authentication/login_attempt_req.vb"
        Try
            Dim app = Await client.GetApplicationAsync(
                "https://api.stormpath.com/v1/applications/$YOUR_APPLICATION_ID")

            Dim loginResult = Await app.AuthenticateAccountAsync(
                "han@newrepublic.gov", "first2shoot!")

            Dim accountDetails = Await loginResult.GetAccountAsync()

        Catch rex As ResourceException
            ' Bad login credentials!
            Console.WriteLine("Error logging in. " + rex.Message)
        End Try
#End Region
    End Function

    Public Async Function LoginAttemptExpanded() As Task
        Dim app As IApplication = Nothing

#Region "code/vbnet/authentication/login_attempt_req_expand_account.vb"
        Dim loginRequest = New UsernamePasswordRequestBuilder() _
            .SetUsernameOrEmail("han@newrepublic.gov") _
            .SetPassword("first2shoot!") _
            .Build()

        Dim loginResult = Await app.AuthenticateAccountAsync(
            loginRequest,
            Function(opt) opt.Expand(Function(result) result.GetAccount()))
#End Region
    End Function


    Public Async Function CustomDataAccountLink() As Task
        Dim account As IAccount = Nothing

#Region "code/vbnet/authentication/customdata_accountlink.vb"
        Dim customData = Await account.GetCustomDataAsync()

        customData.Put("https://api.stormpath.com/v1/accounts/3fLduLKlEx")
        Await customData.SaveAsync()
#End Region
    End Function

    Public Async Function GetAccountStoreMappings() As Task
#Region "code/vbnet/authentication/get_asm_req.vb"
        Dim app = Await client.GetApplicationAsync(
            "https://api.stormpath.com/v1/applications/5nan67mWrYrBmLGu7nGurh")

        Dim accountStoreMappings = Await app _
            .GetAccountStoreMappings() _
            .ToListAsync()
#End Region
    End Function

    Public Async Function CreateDefaultAccountStoreMapping() As Task
#Region "code/vbnet/authentication/create_asm.vb"
        Dim application = Await client.GetApplicationAsync(
            "https://api.stormpath.com/v1/applications/1gk4Dxzi6o4PbdleXaMPL")

        Dim directory = Await client.GetDirectoryAsync(
            "https://api.stormpath.com/v1/directories/2SKhstu8PlaekcaEXampLE")

        Dim newMapping = client.Instantiate(Of IApplicationAccountStoreMapping)() _
            .SetApplication(application) _
            .SetAccountStore(directory) _
            .SetListIndex(0) _
            .SetDefaultAccountStore(True) _
            .SetDefaultGroupStore(True)

        Await application.CreateAccountStoreMappingAsync(newMapping)
#End Region
    End Function

    Public Async Function UpdateExistingMappingListIndex() As Task
        Dim existingMapping As IApplicationAccountStoreMapping = Nothing

#Region "code/vbnet/authentication/change_login_priority.vb"
        existingMapping.SetListIndex(0)
        Await existingMapping.SaveAsync()
#End Region
    End Function

    Public Async Function UpdateExistingMappingDefaultStores() As Task
        Dim existingMapping As IApplicationAccountStoreMapping = Nothing

#Region "code/vbnet/authentication/change_default_stores.vb"
        existingMapping.SetDefaultAccountStore(True)
        existingMapping.SetDefaultGroupStore(True)
        Await existingMapping.SaveAsync()
#End Region
    End Function

    Public Async Function GetOauthPolicy() As Task
        Dim app As IApplication = Nothing

#Region "code/vbnet/authentication/oauth_policy.vb"
        Dim oauthPolicy = Await app.GetOauthPolicyAsync()
#End Region
    End Function

    Public Async Function UpdateOauthPolicyTtls() As Task
        Dim oauthPolicy As IOauthPolicy = Nothing

#Region "code/vbnet/authentication/update_oauth_ttl_req.vb"
        oauthPolicy.SetAccessTokenTimeToLive(TimeSpan.FromMinutes(30))
        oauthPolicy.SetRefreshTokenTimeToLive(TimeSpan.FromDays(7))
        Await oauthPolicy.SaveAsync()
#End Region
    End Function

    Public Async Function OauthPasswordGrantFlow() As Task
        Dim app As IApplication = Nothing

#Region "code/vbnet/authentication/generate_oauth_token_req.vb"
        Dim passwordGrantRequest = OauthRequests.NewPasswordGrantRequest() _
            .SetLogin("han@newrepublic.gov") _
            .SetPassword("first2shoot!") _
            .Build()

        Dim grantResponse = Await app.NewPasswordGrantAuthenticator() _
            .AuthenticateAsync(passwordGrantRequest)
#End Region
    End Function

    Public Async Function OauthValidateAccessToken() As Task
        Dim app As IApplication = Nothing
        Dim access_token As String = Nothing

#Region "code/vbnet/authentication/validate_oauth_token.vb"
        Dim validationRequest = OauthRequests.NewJwtAuthenticationRequest() _
            .SetJwt(access_token) _
            .Build()

        Dim accessToken = Await app.NewJwtAuthenticator() _
            .AuthenticateAsync(validationRequest)
#End Region
    End Function

    Public Async Function OauthValidateAccessTokenLocally() As Task
        Dim app As IApplication = Nothing
        Dim access_token As String = Nothing

#Region "code/vbnet/authentication/validate_oauth_token_local.vb"
        Dim validationRequest = OauthRequests.NewJwtAuthenticationRequest() _
            .SetJwt(access_token) _
            .Build()

        Dim accessToken = Await app.NewJwtAuthenticator() _
            .WithLocalValidation() _
            .AuthenticateAsync(validationRequest)
#End Region
    End Function


    Public Async Function OauthRefreshAccessToken() As Task
        Dim app As IApplication = Nothing
        Dim refresh_token As String = Nothing

#Region "code/vbnet/authentication/refresh_access_token_req.vb"
        Dim refreshGrantRequest = OauthRequests.NewRefreshGrantRequest() _
            .SetRefreshToken(refresh_token) _
            .Build()

        Dim grantResponse = Await app.NewRefreshGrantAuthenticator() _
            .AuthenticateAsync(refreshGrantRequest)
#End Region
    End Function


    Public Async Function GetTokensForAccount() As Task
        Dim account As IAccount = Nothing
        Dim access_token As String = Nothing
        Dim refresh_token As String = Nothing

#Region "code/vbnet/authentication/get_access_tokens.vb"
        Dim allAccessTokens = Await account.GetAccessTokens().ToListAsync()
        Dim allRefreshTokens = Await account.GetRefreshTokens().ToListAsync()

        Dim accessTokenToDelete = allAccessTokens.Where(Function(at) at.Jwt = access_token)
        Dim refreshTokenToDelete = allRefreshTokens.Where(Function(rt) rt.Jwt = refresh_token)
#End Region
    End Function

    Public Async Function GetTokensForAccountForApplication() As Task
        Dim account As IAccount = Nothing

#Region "code/vbnet/authentication/get_access_tokens_for_app.vb"
        Dim allAccessTokens = Await account.GetAccessTokens() _
            .Where(Function(at) at.ApplicationHref = "https://api.stormpath.com/v1/applications/1gk4Dxzi6o4PbdlBVa6tfR") _
            .ToListAsync()
#End Region
    End Function

    Public Async Function DeleteTokens() As Task
        Dim accessTokenToDelete As IAccessToken = Nothing
        Dim refreshTokenToDelete As IRefreshToken = Nothing

#Region "code/vbnet/authentication/delete_user_access_tokens_req.vb"
        Await accessTokenToDelete.DeleteAsync()
        Await refreshTokenToDelete.DeleteAsync()
#End Region
    End Function

    Public Async Function CreateGoogleDirectory() As Task
#Region "code/vbnet/authentication/create_directory_google.vb"
        '# Imports Stormpath.SDK.Provider

        Dim googleDirectory = client.Instantiate(Of IDirectory)()
        googleDirectory.SetName("My Google Directory")

        Await client.CreateDirectoryAsync(
            googleDirectory,
            Function(options) options.ForProvider(client _
                                                  .Providers() _
                                                  .Google() _
                                                  .Builder() _
                                                  .SetClientId("YOUR_GOOGLE_CLIENT_ID") _
                                                  .SetClientSecret("YOUR_GOOGLE_CLIENT_SECRET") _
                                                  .SetRedirectUri("YOUR_GOOGLE_REDIRECT_URI") _
                                                  .Build()))
#End Region
    End Function

    Public Async Function GetGoogleAccountWithCode() As Task
        Dim app As IApplication = Nothing
        Dim code As String = Nothing

#Region "code/vbnet/authentication/create_account_google_providerdata_code.vb"
        Dim request = client.Providers() _
            .Google() _
            .Account() _
            .SetCode(code) _
            .Build()
        Dim result = Await app.GetAccountAsync(request)
#End Region
    End Function

    Public Async Function GetGoogleAccountWithAccessToken() As Task
        Dim app As IApplication = Nothing
        Dim access_token As String = Nothing

#Region "code/vbnet/authentication/create_account_google_providerdata_access_token.vb"
        Dim request = client.Providers() _
            .Google() _
            .Account() _
            .SetAccessToken(access_token) _
            .Build()
        Dim result = Await app.GetAccountAsync(request)
#End Region
    End Function

    Public Async Function CreateFacebookDirectory() As Task
#Region "code/vbnet/authentication/create_directory_fb.vb"
        '# Imports Stormpath.SDK.Provider

        Dim facebookDirectory = client.Instantiate(Of IDirectory)()
        facebookDirectory.SetName("My Facebook Directory")

        Await client.CreateDirectoryAsync(
            facebookDirectory,
            Function(options) options.ForProvider(client _
                                                  .Providers() _
                                                  .Facebook() _
                                                  .Builder() _
                                                  .SetClientId("YOUR_FACEBOOK_APP_ID") _
                                                  .SetClientSecret("YOUR_FACEBOOK_APP_SECRET") _
                                                  .Build()))
#End Region
    End Function

    Public Async Function GetFacebookAccountWithAccessToken() As Task
        Dim app As IApplication = Nothing
        Dim access_token As String = Nothing

#Region "code/vbnet/authentication/create_account_fb_providerdata_access_token.vb"
        Dim request = client.Providers() _
            .Facebook() _
            .Account() _
            .SetAccessToken(access_token) _
            .Build()
        Dim result = Await app.GetAccountAsync(request)
#End Region
    End Function


    Public Async Function CreateGithubkDirectory() As Task
#Region "code/vbnet/authentication/create_directory_github.vb"
        '# Imports Stormpath.SDK.Provider

        Dim githubDirectory = client.Instantiate(Of IDirectory)()
        githubDirectory.SetName("My Github Directory")

        Await client.CreateDirectoryAsync(
            githubDirectory,
            Function(options) options.ForProvider(client _
                                                  .Providers() _
                                                  .Github() _
                                                  .Builder() _
                                                  .SetClientId("YOUR_GITHUB_CLIENT_ID") _
                                                  .SetClientSecret("YOUR_GITHUB_CLIENT_SECRET") _
                                                  .Build()))
#End Region
    End Function

    Public Async Function GetGithubAccountWithAccessToken() As Task
        Dim app As IApplication = Nothing
        Dim access_token As String = Nothing

#Region "code/vbnet/authentication/create_account_github_providerdata_access_token.vb"
        Dim request = client.Providers() _
            .Github() _
            .Account() _
            .SetAccessToken(access_token) _
            .Build()
        Dim result = Await app.GetAccountAsync(request)
#End Region
    End Function

    Public Async Function CreateLinkedInDirectory() As Task
#Region "code/vbnet/authentication/create_directory_linkedin.vb"
        '# Imports Stormpath.SDK.Provider

        Dim linkedInDirectory = client.Instantiate(Of IDirectory)()
        linkedInDirectory.SetName("My LinkedIn Directory")

        Await client.CreateDirectoryAsync(
            linkedInDirectory,
            Function(options) options.ForProvider(client _
                                                  .Providers() _
                                                  .LinkedIn() _
                                                  .Builder() _
                                                  .SetClientId("YOUR_LINKEDIN_APP_ID") _
                                                  .SetClientSecret("YOUR_LINKEDIN_APP_SECRET") _
                                                  .Build()))
#End Region
    End Function

    Public Async Function GetLinkedInAccountWithAccessToken() As Task
        Dim app As IApplication = Nothing
        Dim access_token As String = Nothing

#Region "code/vbnet/authentication/create_account_linkedin_providerdata_access_token.vb"
        Dim request = client.Providers() _
            .LinkedIn() _
            .Account() _
            .SetAccessToken(access_token) _
            .Build()
        Dim result = Await app.GetAccountAsync(request)
#End Region
    End Function

    Public Async Function GetSamlPolicy() As Task
        Dim app As IApplication = Nothing

#Region "code/vbnet/authentication/saml_policy_example.vb"
        Dim samlPolicy = Await app.GetSamlPolicyAsync()

        Dim samlServiceProvider = Await samlPolicy.GetSamlServiceProviderAsync()
        ' samlServiceProvider.Href is the SAML Service Provider URL

        Dim ssoInitiationEndpoint = Await samlServiceProvider.GetSsoInitiationEndpointAsync()
        ' ssoInitiationEndpoint.Href is the SSO Initiation Endpoint
#End Region
    End Function

    Public Async Function CreateApiKeyForAccount() As Task
        Dim account As IAccount = Nothing

#Region "code/vbnet/authentication/create_apikey_req.vb"
        Dim newApiKey = Await account.CreateApiKeyAsync()
#End Region
    End Function

    Public Async Function DeleteApiKey() As Task
        Dim app As IApplication = Nothing
        Dim api_key_id As String = Nothing

#Region "code/vbnet/authentication/delete_apikey.vb"
        Dim apiKey = Await app.GetApiKeyAsync(api_key_id)
        Await apiKey.DeleteAsync()
#End Region
    End Function

    Public Async Function DisableApiKey() As Task
        Dim app As IApplication = Nothing
        Dim api_key_id As String = Nothing

#Region "code/vbnet/authentication/disable_apikey.vb"
        Dim apiKey = Await app.GetApiKeyAsync(api_key_id)
        apiKey.SetStatus(ApiKeyStatus.Disabled)
        Await apiKey.SaveAsync()
#End Region
    End Function

    Public Async Function AuthenticateBasicApiRequest() As Task
        Dim app As IApplication = Nothing
        Dim api_key_id As String = Nothing
        Dim api_key_secret As String = Nothing

#Region "code/vbnet/authentication/authenticate_basic_req.vb"
        Dim apiKeyAuthRequest = New ApiKeyRequestBuilder() _
            .SetId(api_key_id) _
            .SetSecret(api_key_secret) _
            .Build()

        Dim result = Await app.AuthenticateAccountAsync(apiKeyAuthRequest)
        Dim account = Await result.GetAccountAsync()
#End Region
    End Function

End Class
