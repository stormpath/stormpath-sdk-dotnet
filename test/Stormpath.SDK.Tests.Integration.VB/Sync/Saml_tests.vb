' <copyright file="Saml_tests.vb" company="Stormpath, Inc.">
' Copyright (c) 2016 Stormpath, Inc.
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'      http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' </copyright>

Imports Shouldly
Imports Stormpath.SDK.Sync
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Sync
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Saml_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Sub Getting_sso_initiation_endpoint(clientBuilder As TestClientProvider)
            Dim client = clientBuilder.GetClient()
            Dim defaultApplication = client.GetApplications() _
                .Where(Function(x) x.Name = "My Application") _
                .Synchronously() _
                .SingleOrDefault()

            Dim samlPolicy = defaultApplication.GetSamlPolicy()
            samlPolicy.Href.ShouldNotBeNullOrEmpty()

            Dim samlProvider = samlPolicy.GetSamlServiceProvider()
            samlProvider.Href.ShouldNotBeNullOrEmpty()

            Dim ssoInitiationEndpoint = samlProvider.GetSsoInitiationEndpoint()
            Dim endpointUrl = ssoInitiationEndpoint.Href
            endpointUrl.ShouldNotBeNullOrEmpty()
            endpointUrl.ShouldContain("/saml/sso/idpRedirect")
        End Sub
    End Class
End Namespace