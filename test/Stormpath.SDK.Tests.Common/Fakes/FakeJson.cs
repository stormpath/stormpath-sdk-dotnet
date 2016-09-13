// <copyright file="FakeJson.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Tests.Common.Fakes
{
#pragma warning disable SA1124 // Do not use regions
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

        public static readonly string AccountWithExpandedCustomData = @"
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
        ""createdAt"": ""2015-07-30T02:42:17.833Z"",
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/customData"",
        ""modifiedAt"": ""2015-07-30T02:42:17.833Z"",
        ""isAdmin"": false
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

        public static readonly string AccountWithExpandedGroups = @"
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
        ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant/groups"",
        ""items"": [
            {
                ""accountMemberships"": {
                    ""href"": ""https://api.stormpath.com/v1/groups/group1/accountMemberships""
                },
                ""accounts"": {
                    ""href"": ""https://api.stormpath.com/v1/groups/group1/accounts""
                },
                ""applications"": {
                    ""href"": ""https://api.stormpath.com/v1/groups/group1/applications""
                },
                ""createdAt"": ""2015-08-24T17:02:52.915Z"",
                ""customData"": {
                    ""href"": ""https://api.stormpath.com/v1/groups/group1/customData""
                },
                ""description"": ""Those loyal to the Galactic Empire."",
                ""directory"": {
                    ""href"": ""https://api.stormpath.com/v1/directories/directory1""
                },
                ""href"": ""https://api.stormpath.com/v1/groups/group1"",
                ""modifiedAt"": ""2015-08-24T17:02:52.915Z"",
                ""name"": ""Imperials"",
                ""status"": ""DISABLED"",
                ""tenant"": {
                    ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
                }
            },
            {
                ""accountMemberships"": {
                    ""href"": ""https://api.stormpath.com/v1/groups/group2/accountMemberships""
                },
                ""accounts"": {
                    ""href"": ""https://api.stormpath.com/v1/groups/group2/accounts""
                },
                ""applications"": {
                    ""href"": ""https://api.stormpath.com/v1/groups/group2/applications""
                },
                ""createdAt"": ""2015-08-24T17:02:42.755Z"",
                ""customData"": {
                    ""href"": ""https://api.stormpath.com/v1/groups/group2/customData""
                },
                ""description"": ""The members of the Rebel Alliance."",
                ""directory"": {
                    ""href"": ""https://api.stormpath.com/v1/directories/directory1""
                },
                ""href"": ""https://api.stormpath.com/v1/groups/group2"",
                ""modifiedAt"": ""2015-08-24T17:02:42.755Z"",
                ""name"": ""Rebels"",
                ""status"": ""ENABLED"",
                ""tenant"": {
                    ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
                }
            }
        ],
        ""limit"": 25,
        ""offset"": 0,
        ""size"": 2
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

        #region Account list

        public static readonly string AccountList = @"
{
    ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant/accounts"",
    ""items"": [
        {
            ""accessTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account1/accessTokens""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account1/apiKeys""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account1/applications""
            },
            ""createdAt"": ""2015-07-21T23:50:49.078Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account1/customData""
            },
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""email"": ""han.solo@corellia.core"",
            ""emailVerificationToken"": null,
            ""fullName"": ""Han Solo"",
            ""givenName"": ""Han"",
            ""groupMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account1/groupMemberships""
            },
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account1/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/accounts/account1"",
            ""middleName"": null,
            ""modifiedAt"": ""2015-07-21T23:50:49.078Z"",
            ""providerData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account1/providerData""
            },
            ""refreshTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account1/refreshTokens""
            },
            ""status"": ""ENABLED"",
            ""surname"": ""Solo"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""username"": ""han.solo@corellia.core""
        },
        {
            ""accessTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account2/accessTokens""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account2/apiKeys""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account2/applications""
            },
            ""createdAt"": ""2015-07-30T14:46:53.228Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account2/customData""
            },
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""email"": ""lskywalker@tattooine.rim"",
            ""emailVerificationToken"": null,
            ""fullName"": ""Luke Skywalker"",
            ""givenName"": ""Luke"",
            ""groupMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account2/groupMemberships""
            },
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account2/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/accounts/account2"",
            ""middleName"": null,
            ""modifiedAt"": ""2015-07-30T14:46:53.228Z"",
            ""providerData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account2/providerData""
            },
            ""refreshTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account2/refreshTokens""
            },
            ""status"": ""ENABLED"",
            ""surname"": ""Skywalker"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""username"": ""lskywalker@tattooine.rim""
        },
        {
            ""accessTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account3/accessTokens""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account3/apiKeys""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account3/applications""
            },
            ""createdAt"": ""2015-08-06T17:50:32.987Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account3/customData""
            },
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""email"": ""leia.organa@alderaan.core"",
            ""emailVerificationToken"": null,
            ""fullName"": ""Leia Organa"",
            ""givenName"": ""Leia"",
            ""groupMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account3/groupMemberships""
            },
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account3/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/accounts/account3"",
            ""middleName"": null,
            ""modifiedAt"": ""2015-08-06T17:50:32.987Z"",
            ""providerData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account3/providerData""
            },
            ""refreshTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account3/refreshTokens""
            },
            ""status"": ""ENABLED"",
            ""surname"": ""Organa"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""username"": ""leia.organa@alderaan.core""
        },
        {
            ""accessTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account4/accessTokens""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account4/apiKeys""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account4/applications""
            },
            ""createdAt"": ""2015-07-29T23:36:57.466Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account4/customData""
            },
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""email"": ""chewbacca@kashyyyk.rim"",
            ""emailVerificationToken"": null,
            ""fullName"": ""Chewbacca"",
            ""givenName"": ""Chewbacca"",
            ""groupMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account4/groupMemberships""
            },
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account4/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/accounts/account4"",
            ""middleName"": null,
            ""modifiedAt"": ""2015-07-29T23:36:57.466Z"",
            ""providerData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account4/providerData""
            },
            ""refreshTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account4/refreshTokens""
            },
            ""status"": ""ENABLED"",
            ""surname"": null,
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""username"": ""chewbacca@kashyyk.rim""
        },
        {
            ""accessTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account5/accessTokens""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account5/apiKeys""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account5/applications""
            },
            ""createdAt"": ""2015-07-29T23:30:11.151Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account5/customData""
            },
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""email"": ""lcalrissian@socorro.rim"",
            ""emailVerificationToken"": null,
            ""fullName"": ""Lando Calrissian"",
            ""givenName"": ""Lando"",
            ""groupMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account5/groupMemberships""
            },
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account5/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/accounts/account5"",
            ""middleName"": null,
            ""modifiedAt"": ""2015-07-29T23:30:11.151Z"",
            ""providerData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account5/providerData""
            },
            ""refreshTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account5/refreshTokens""
            },
            ""status"": ""ENABLED"",
            ""surname"": ""Calrissian"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""username"": ""lcalrissian@socorro.rim""
        },
        {
            ""accessTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account6/accessTokens""
            },
            ""apiKeys"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account6/apiKeys""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account6/applications""
            },
            ""createdAt"": ""2015-07-30T02:42:17.833Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account6/customData""
            },
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""email"": ""ackbar@dac.rim"",
            ""emailVerificationToken"": null,
            ""fullName"": ""Gial Ackbar"",
            ""givenName"": ""Gial"",
            ""groupMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account6/groupMemberships""
            },
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account6/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/accounts/account6"",
            ""middleName"": null,
            ""modifiedAt"": ""2015-07-30T02:42:17.833Z"",
            ""providerData"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account6/providerData""
            },
            ""refreshTokens"": {
                ""href"": ""https://api.stormpath.com/v1/accounts/account6/refreshTokens""
            },
            ""status"": ""ENABLED"",
            ""surname"": ""Ackbar"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            },
            ""username"": ""micah@stormpath.com""
        }
    ],
    ""limit"": 25,
    ""offset"": 0,
    ""size"": 6
}";

        #endregion

        #region Account store mapping

        public static readonly string AccountStoreMapping = @"
{
    ""accountStore"": {
        ""href"": ""https://api.stormpath.com/v1/directories/directory1""
    },
    ""application"": {
        ""href"": ""https://api.stormpath.com/v1/applications/foobarApplication""
    },
    ""href"": ""https://api.stormpath.com/v1/accountStoreMappings/foobarASM"",
    ""isDefaultAccountStore"": true,
    ""isDefaultGroupStore"": true,
    ""listIndex"": 0
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
            ""authorizedCallbackUris"": [""https://foo.bar/1"", ""https://foo.bar/2""],
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

        #region Directory 'Jedi Council Directory'

        public static readonly string Directory = @"
    {
            ""accountCreationPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/accountCreationPolicies/directory1""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/accounts""
            },
            ""applicationMappings"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/applicationMappings""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/applications""
            },
            ""createdAt"": ""2015-07-21T23:50:49.569Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/customData""
            },
            ""description"": ""The members of the Jedi Council."",
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/directories/directory1"",
            ""modifiedAt"": ""2015-07-21T23:50:49.569Z"",
            ""name"": ""Jedi Council Directory"",
            ""passwordPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/passwordPolicies/directory1""
            },
            ""provider"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/provider""
            },
            ""status"": ""ENABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            }
    }";

        #endregion

        #region Directory list

        public static readonly string DirectoryList = @"
{
    ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant/directories"",
    ""items"": [
        {
            ""accountCreationPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/accountCreationPolicies/directory1""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/accounts""
            },
            ""applicationMappings"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/applicationMappings""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/applications""
            },
            ""createdAt"": ""2015-07-21T23:50:49.569Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/customData""
            },
            ""description"": ""The members of the Jedi Council."",
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/directories/directory1"",
            ""modifiedAt"": ""2015-07-21T23:50:49.569Z"",
            ""name"": ""Jedi Council Directory"",
            ""passwordPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/passwordPolicies/directory1""
            },
            ""provider"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1/provider""
            },
            ""status"": ""ENABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            }
        },
        {
            ""accountCreationPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/accountCreationPolicies/directory2""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory2/accounts""
            },
            ""applicationMappings"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory2/applicationMappings""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory2/applications""
            },
            ""createdAt"": ""2015-07-21T23:50:49.064Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory2/customData""
            },
            ""description"": ""Default directory for accounts and groups that may access Stormpath IAM."",
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory2/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/directories/directory2"",
            ""modifiedAt"": ""2015-07-21T23:50:49.064Z"",
            ""name"": ""Stormpath Administrators"",
            ""passwordPolicy"": {
                ""href"": ""https://api.stormpath.com/v1/passwordPolicies/directory2""
            },
            ""provider"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory2/provider""
            },
            ""status"": ""ENABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            }
        }
    ],
    ""limit"": 25,
    ""offset"": 0,
    ""size"": 2
}";

        #endregion

        #region Group 'Rebels'

        public static readonly string Group = @"
        {
            ""accountMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group1/accountMemberships""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group1/accounts""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group1/applications""
            },
            ""createdAt"": ""2015-08-24T17:02:52.915Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group1/customData""
            },
            ""description"": ""Those loyal to the Galactic Empire."",
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""href"": ""https://api.stormpath.com/v1/groups/group1"",
            ""modifiedAt"": ""2015-08-24T17:02:52.915Z"",
            ""name"": ""Imperials"",
            ""status"": ""DISABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            }
        }";

        #endregion

        #region Group list

        public static readonly string GroupList = @"
{
    ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant/groups"",
    ""items"": [
        {
            ""accountMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group1/accountMemberships""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group1/accounts""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group1/applications""
            },
            ""createdAt"": ""2015-08-24T17:02:52.915Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group1/customData""
            },
            ""description"": ""Those loyal to the Galactic Empire."",
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""href"": ""https://api.stormpath.com/v1/groups/group1"",
            ""modifiedAt"": ""2015-08-24T17:02:52.915Z"",
            ""name"": ""Imperials"",
            ""status"": ""DISABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            }
        },
        {
            ""accountMemberships"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group2/accountMemberships""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group2/accounts""
            },
            ""applications"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group2/applications""
            },
            ""createdAt"": ""2015-08-24T17:02:42.755Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/groups/group2/customData""
            },
            ""description"": ""The members of the Rebel Alliance."",
            ""directory"": {
                ""href"": ""https://api.stormpath.com/v1/directories/directory1""
            },
            ""href"": ""https://api.stormpath.com/v1/groups/group2"",
            ""modifiedAt"": ""2015-08-24T17:02:42.755Z"",
            ""name"": ""Rebels"",
            ""status"": ""ENABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            }
        }
    ],
    ""limit"": 25,
    ""offset"": 0,
    ""size"": 2
}";

        #endregion

        #region Group membership

        public static readonly string GroupMembership = @"
{
    ""account"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount""
    },
    ""group"": {
        ""href"": ""https://api.stormpath.com/v1/groups/foobarGroup""
    },
    ""href"": ""https://api.stormpath.com/v1/groupMemberships/foobarGroupMembership""
}";

        #endregion

        #region CustomData

        public static readonly string CustomData = @"
        {
            ""createdAt"": ""2015-08-12T21:30:53.321Z"",
            ""expiresAt"": ""2015-08-15T21:31:29Z"",
            ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount/customData"",
            ""membershipType"": ""lifetime"",
            ""modifiedAt"": ""2015-08-12T21:31:49.854Z""
        }";

        #endregion

        #region Organization 'Star Wars'

        public static readonly string Organizatino = @"
        {
            ""accountStoreMappings"": {
                ""href"": ""https://api.stormpath.com/v1/organizations/org1/accountStoreMappings""
            },
            ""accounts"": {
                ""href"": ""https://api.stormpath.com/v1/organizations/org1/accounts""
            },
            ""createdAt"": ""2015-12-11T22:23:01.014Z"",
            ""customData"": {
                ""href"": ""https://api.stormpath.com/v1/organizations/org1/customData""
            },
            ""defaultAccountStoreMapping"": null,
            ""defaultGroupStoreMapping"": null,
            ""description"": ""A long time ago, in a galaxy far away..."",
            ""groups"": {
                ""href"": ""https://api.stormpath.com/v1/organizations/org1/groups""
            },
            ""href"": ""https://api.stormpath.com/v1/organizations/org1"",
            ""modifiedAt"": ""2015-12-11T22:23:01.014Z"",
            ""name"": ""Star Wars"",
            ""nameKey"": ""star-wars"",
            ""status"": ""ENABLED"",
            ""tenant"": {
                ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
            }
        }";

        #endregion

        #region API Key

        public static readonly string ApiKey = @"
{
    ""account"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount""
    },
    ""href"": ""https://api.stormpath.com/v1/apiKeys/83JFN57290NFKDHENXEXAMPLE"",
    ""id"": ""83JFN57290NFKDHENXEXAMPLE"",
    ""secret"": ""asdfqwerty1234567890/ASDFQWERTY09876example"",
    ""status"": ""ENABLED"",
    ""tenant"": {
        ""href"": ""https://api.stormpath.com/v1/tenants/foobarTenant""
    }
}";

        #endregion

        #region Providers

        public static readonly string SamlProvider = @"
{
   ""href"":""https://api.stormpath.com/v1/directories/directory1/provider"",
   ""createdAt"":""2016-05-16T18:59:59.183Z"",
   ""modifiedAt"":""2016-05-16T18:59:59.183Z"",
   ""providerId"":""saml"",
   ""ssoLoginUrl"":""https://test.foo.bar/trust/saml2/http-post/sso/12345"",
   ""ssoLogoutUrl"":""https://test.foo.bar/trust/saml2/http-post/slo/12345"",
   ""encodedX509SigningCert"":""-----BEGIN CERTIFICATE-----fakefakefake\n-----END CERTIFICATE-----"",
   ""requestSignatureAlgorithm"":""RSA-SHA1"",
   ""attributeStatementMappingRules"":{
      ""href"":""https://api.stormpath.com/v1/attributeStatementMappingRules/foobarAsm1""
   },
   ""serviceProviderMetadata"":{
      ""href"":""https://api.stormpath.com/v1/samlServiceProviderMetadatas/foobarSpm1""
   }
}";

        #endregion
    }
#pragma warning restore SA1124 // Do not use regions
}