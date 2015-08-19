// <copyright file="FakeJson.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

namespace Stormpath.SDK.Tests.Fakes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "Reviewed")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Reviewed")]
    public static class FakeJson
    {
        #region Tenant foo-bar

        public static readonly string Tenant = @"
{
    ""accounts"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar/accounts""
    },
    ""agents"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar/agents""
    },
    ""applications"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar/applications""
    },
    ""createdAt"": ""2015-07-21T23:50:49.058Z"",
    ""customData"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar/customData""
    },
    ""directories"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar/directories""
    },
    ""groups"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar/groups""
    },
    ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar"",
    ""idSites"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar/idSites""
    },
    ""key"": ""foo-bar"",
    ""modifiedAt"": ""2015-07-21T23:50:49.579Z"",
    ""name"": ""foo-bar"",
    ""organizations"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foo-bar/organizations""
    }
}";

        #endregion

        #region Account han.solo@corellia.core

        public static readonly string Account = @"
{
    ""accessTokens"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/accessTokens""
    },
    ""apiKeys"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/apiKeys""
    },
    ""applications"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/applications""
    },
    ""createdAt"": ""2015-07-21T23:50:49.078Z"",
    ""customData"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/customData""
    },
    ""directory"": {
        ""href"": ""https://api.stormpath.com/v1/directories/foobarDirectory""
    },
    ""email"": ""han.solo@corellia.core"",
    ""emailVerificationToken"": null,
    ""fullName"": ""Han Solo"",
    ""givenName"": ""Han"",
    ""groupMemberships"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/groupMemberships""
    },
    ""groups"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/groups""
    },
    ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount"",
    ""middleName"": null,
    ""modifiedAt"": ""2015-07-21T23:50:49.078Z"",
    ""providerData"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/providerData""
    },
    ""refreshTokens"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/refreshTokens""
    },
    ""status"": ""ENABLED"",
    ""surname"": ""Solo"",
    ""tenant"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
    },
    ""username"": ""han.solo@corellia.core""
}";

        #endregion

    }
}