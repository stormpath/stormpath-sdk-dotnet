Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Client
Imports Stormpath.SDK.Http
Imports Stormpath.SDK.IdSite
Imports Stormpath.SDK.Oauth

Namespace DocExamplesVB.ProductGuide

    Public Class IdSite

        Public Async Function BuildIdSiteUrl() As Task
            Dim client As IClient = Nothing

#Region "code/vbnet/idsite/build_idsite_url.vb"

            Dim app = Await client.GetApplicationAsync("application_url")

            Dim idSiteUrl = app.NewIdSiteUrlBuilder() _
                .SetCallbackUri("http://mysite.foo/idsiteCallback") _
                .Build()

#End Region
        End Function

        Public Async Function ConsumeIdSiteAssertion() As Task
            Dim app As IApplication = Nothing

#Region "code/vbnet/idsite/build_idsite_url.vb"

            Dim requestDescriptor = HttpRequests.NewRequestDescriptor() _
                .WithMethod("GET") _
                .WithUri("incoming_uri") _
                .Build()

            Dim idSiteListener = app.NewIdSiteAsyncCallbackHandler(requestDescriptor)

            Dim accountResult = Await idSiteListener.GetAccountResultAsync()

#End Region
        End Function

        Public Async Function ExchangeIdSiteAssertionForToken() As Task
            Dim app As IApplication = Nothing
            dim incoming_uri as String = nothing

#Region "code/vbnet/idsite/jwt_for_oauth_req.vb"

            Dim jwt = incoming_uri.Split("="c)(1)

            Dim idSiteTokenExchangeRequest = OauthRequests _
                .NewIdSiteTokenAuthenticationRequest() _
                .SetJwt(jwt) _
                .Build()

            Dim grantResponse = Await app.NewIdSiteTokenAuthenticator() _
                .AuthenticateAsync(idSiteTokenExchangeRequest)

            ' Token is stored in grantResponse.AccessTokenString
#End Region
        End Function

        Public Sub BuildLogoutUrl()
            Dim app As IApplication = Nothing

#Region "code/vbnet/idsite/logout_from_idsite_req.vb"

            Dim logoutUrl = app.NewIdSiteUrlBuilder() _
                .SetCallbackUri("http://mysite.foo/idsiteCallback") _
                .ForLogout() _
                .Build()

#End Region
        End Sub

        Public Sub ConsumeLogout()
            Dim accountResult As IAccountResult = Nothing

            #Region "code/vbnet/idsite/logout_from_idsite_resp.vb"

            ' In your callback handler, after GetAccountResultAsync()

            If accountResult.Status = IdSiteResultStatus.Logout Then
                ' This was a logout! Proceed accordingly...
            End If

            #End Region
        End Sub
        
Public Sub BuildPasswordResetUrl()
	Dim app As IApplication = Nothing
           Dim sptoken_from_url As String = nothing

	#Region "code/vbnet/idsite/idsite_reset_pwd.vb"

	Dim logoutUrl = app.NewIdSiteUrlBuilder() _
                .SetCallbackUri("http://mysite.foo/idsiteCallback") _
                .SetPath("/#/reset") _
                .SetSpToken(sptoken_from_url) _
                .Build()

	#End Region
End Sub

    End Class
End Namespace
