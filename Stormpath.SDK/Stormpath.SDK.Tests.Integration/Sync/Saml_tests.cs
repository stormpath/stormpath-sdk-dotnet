// <copyright file="Saml_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Linq;
using Shouldly;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Integration;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Sync
{
    [Collection(nameof(IntegrationTestCollection))]
    public class Saml_tests
    {
        private readonly TestFixture fixture;

        public Saml_tests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestClients.GetClients), MemberType = typeof(TestClients))]
        public void Getting_sso_initiation_endpoint(TestClientProvider clientBuilder)
        {
            var client = clientBuilder.GetClient();
            var defaultApplication = client.GetApplications()
                .Where(x => x.Name == "My Application")
                .Synchronously()
                .SingleOrDefault();

            var samlPolicy = defaultApplication.GetSamlPolicy();
            samlPolicy.Href.ShouldNotBeNullOrEmpty();

            var samlProvider = samlPolicy.GetSamlServiceProvider();
            samlProvider.Href.ShouldNotBeNullOrEmpty();

            var ssoInitiationEndpoint = samlProvider.GetSsoInitiationEndpoint();
            var endpointUrl = ssoInitiationEndpoint.Href;
            endpointUrl.ShouldNotBeNullOrEmpty();
            endpointUrl.ShouldContain("/saml/sso/idpRedirect");
        }
    }
}
