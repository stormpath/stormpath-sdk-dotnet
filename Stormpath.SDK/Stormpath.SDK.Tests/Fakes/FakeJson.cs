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

        #region Application 'Lightsabers Galore'

        public static readonly string Application = @"
    {
            ""accountStoreMappings"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/accountStoreMappings""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/accounts""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/apiKeys""
            },
            ""authTokens"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/authTokens""
            },
            ""createdAt"": ""2015-07-21T23:50:49.563Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/customData""
            },
            ""defaultAccountStoreMapping"": {
                ""href"": ""https://api.stormpath.com/v1/accountStoreMappings/foobarASM""
            },
            ""defaultGroupStoreMapping"": {
                ""href"": ""https://api.stormpath.com/v1/accountStoreMappings/foobarASM""
            },
            ""description"": ""This application is so awesome, you don't even know."",
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication"",
            ""loginAttempts"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/loginAttempts""
            },
            ""modifiedAt"": ""2015-07-21T23:50:49.622Z"",
            ""name"": ""Lightsabers Galore"",
            ""oAuthPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/oAuthPolicies/foobarApplication""
            },
            ""passwordResetTokens"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/passwordResetTokens""
            },
            ""status"": ""ENABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""verificationEmails"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/verificationEmails""
            }
        }";

        #endregion

        #region Application list

        public static readonly string ApplicationList = @"
{
    ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant/applications"",
    ""items"": [
        {
            ""accountStoreMappings"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication/accountStoreMappings""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1/accounts""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1/apiKeys""
            },
            ""authTokens"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1/authTokens""
            },
            ""createdAt"": ""2015-07-21T23:50:49.563Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1/customData""
            },
            ""defaultAccountStoreMapping"": {
                ""href"": ""https://api.stormpath.com/v1/accountStoreMappings/foobarASM1""
            },
            ""defaultGroupStoreMapping"": {
                ""href"": ""https://api.stormpath.com/v1/accountStoreMappings/foobarASM1""
            },
            ""description"": ""This application was automatically created for you in Stormpath for use with our Quickstart guides(https://docs.stormpath.com). It does apply to your subscription's number of reserved applications and can be renamed or reused for your own purposes."",
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1"",
            ""loginAttempts"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1/loginAttempts""
            },
            ""modifiedAt"": ""2015-07-21T23:50:49.622Z"",
            ""name"": ""My Application"",
            ""oAuthPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/oAuthPolicies/foobarApplication1""
            },
            ""passwordResetTokens"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1/passwordResetTokens""
            },
            ""status"": ""ENABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""verificationEmails"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication1/verificationEmails""
            }
        },
        {
            ""accountStoreMappings"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/accountStoreMappings""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/accounts""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/apiKeys""
            },
            ""authTokens"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/authTokens""
            },
            ""createdAt"": ""2015-07-21T23:50:49.079Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/customData""
            },
            ""defaultAccountStoreMapping"": null,
            ""defaultGroupStoreMapping"": null,
            ""description"": ""Manages access to the Stormpath Administrator Console and API. This application does not apply to your subscription's number of reserved applications."",
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2"",
            ""loginAttempts"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/loginAttempts""
            },
            ""modifiedAt"": ""2015-07-21T23:50:49.083Z"",
            ""name"": ""Stormpath"",
            ""oAuthPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/oAuthPolicies/foobarApplication2""
            },
            ""passwordResetTokens"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/passwordResetTokens""
            },
            ""status"": ""ENABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""verificationEmails"": {
                ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication2/verificationEmails""
            }
        }
    ],
    ""limit"": 25,
    ""offset"": 0,
    ""size"": 2
}";

        #endregion
    }
}