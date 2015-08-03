// <copyright file="ClientApiKeysBuilderTests.cs" company="Stormpath, Inc.">
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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Api;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Tests.SDK
{
    [TestClass]
    public class ClientApiKeysBuilderTests
    {
        [TestClass]
        public class Building_With_Missing_Values
        {
            [TestMethod]
            [ExpectedException(typeof(ApplicationException))]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_missing_id_throws_error()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .SetSecret("foo")
                    .Build();
            }

            [TestMethod]
            [ExpectedException(typeof(ApplicationException))]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_missing_secret_throws_error()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .SetId("foo")
                    .Build();
            }
        }

        [TestClass]
        public class Building_With_Default_Properties_File
        {
            private readonly string defaultLocation =
                System.IO.Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), ".stormpath\\", "apiKey.properties");

            private readonly string fileContents =
                "apiKey.id = 144JVZINOF5EBNCMG9EXAMPLE\r\n" +
                "apiKey.secret = lWxOiKqKPNwJmSldbiSkEbkNjgh2uRSNAb+AEXAMPLE";

            private IClientApiKeyBuilder builder;
            private IFile file;

            [TestInitialize]
            public void Setup()
            {
                this.file = Substitute.For<IFile>();
                this.file.ReadAllText(defaultLocation)
                    .Returns(fileContents);

                this.builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), Substitute.For<IEnvironment>(), file);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_default_properties_file()
            {
                var clientApiKey = this.builder.Build();

                clientApiKey.GetId().ShouldBe("144JVZINOF5EBNCMG9EXAMPLE");
                clientApiKey.GetSecret().ShouldBe("lWxOiKqKPNwJmSldbiSkEbkNjgh2uRSNAb+AEXAMPLE");
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Default_properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = this.builder
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                clientApiKey.GetId().ShouldBe("different");
                clientApiKey.GetSecret().ShouldBe("also_different");
            }
        }

        [TestClass]
        public class Building_With_Environment_Variable_File
        {
            private readonly string envVariableName = "STORMPATH_API_KEY_FILE";
            private readonly string testLocation = "envfile.properties";
            private readonly string fileContents =
                "apiKey.id = envId\r\n" +
                "apiKey.secret = envSecret\r\n";

            private IClientApiKeyBuilder builder;
            private IEnvironment env;
            private IFile file;

            [TestInitialize]
            public void Setup()
            {
                this.env = Substitute.For<IEnvironment>();
                this.env.GetEnvironmentVariable(envVariableName)
                    .Returns(testLocation);

                this.file = Substitute.For<IFile>();
                this.file.ReadAllText(testLocation)
                    .Returns(fileContents);

                this.builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), env, file);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file_from_env_variable()
            {
                var clientApiKey = this.builder.Build();

                clientApiKey.GetId().ShouldBe("envId");
                clientApiKey.GetSecret().ShouldBe("envSecret");
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Env_variable_properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = this.builder
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                clientApiKey.GetId().ShouldBe("different");
                clientApiKey.GetSecret().ShouldBe("also_different");
            }
        }

        [TestClass]
        public class Building_With_Environment_Variable_Values
        {
            private readonly string idVariableName = "STORMPATH_API_KEY_ID";
            private readonly string fakeApiKeyId = "idSetByEnv";
            private readonly string secretVariableName = "STORMPATH_API_KEY_SECRET";
            private readonly string fakeApiSecretId = "secretSetByEnv";

            private IClientApiKeyBuilder builder;
            private IEnvironment env;

            [TestInitialize]
            public void Setup()
            {
                this.env = Substitute.For<IEnvironment>();
                this.env.GetEnvironmentVariable(idVariableName)
                    .Returns(fakeApiKeyId);
                this.env.GetEnvironmentVariable(secretVariableName)
                    .Returns(fakeApiSecretId);

                this.builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), env, Substitute.For<IFile>());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_values_from_env_variables()
            {
                var clientApiKey = this.builder
                    .Build();

                clientApiKey.GetId().ShouldBe(fakeApiKeyId);
                clientApiKey.GetSecret().ShouldBe(fakeApiSecretId);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Env_variable_values_are_lower_priority_than_explicit()
            {
                var clientApiKey = this.builder
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                clientApiKey.GetId().ShouldBe("different");
                clientApiKey.GetSecret().ShouldBe("also_different");
            }
        }

        [TestClass]
        public class Building_With_AppConfig_File
        {
            private readonly string configVariableName = "STORMPATH_API_KEY_FILE";
            private readonly string testLocation = "appConfig.properties";
            private readonly string fileContents =
                "apiKey.id = fromAppConfig?\r\n" +
                "apiKey.secret = fooSecret!\r\n";

            private IClientApiKeyBuilder builder;
            private IConfigurationManager config;
            private IFile file;

            [TestInitialize]
            public void Setup()
            {
                this.config = Substitute.For<IConfigurationManager>();
                this.config.AppSettings.Returns(new System.Collections.Specialized.NameValueCollection()
                {
                    { configVariableName, testLocation }
                });

                this.file = Substitute.For<IFile>();
                this.file.ReadAllText(testLocation)
                    .Returns(fileContents);

                this.builder = new DefaultClientApiKeyBuilder(config, Substitute.For<IEnvironment>(), file);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file_from_app_config()
            {
                var clientApiKey = this.builder.Build();

                clientApiKey.GetId().ShouldBe("fromAppConfig?");
                clientApiKey.GetSecret().ShouldBe("fooSecret!");
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void App_config_properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = this.builder
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                clientApiKey.GetId().ShouldBe("different");
                clientApiKey.GetSecret().ShouldBe("also_different");
            }
        }

        [TestClass]
        public class Building_With_AppConfig_Values
        {
            private readonly string idVariableName = "STORMPATH_API_KEY_ID";
            private readonly string fakeApiKeyId = "idSetByAppConfig";
            private readonly string secretVariableName = "STORMPATH_API_KEY_SECRET";
            private readonly string fakeApiSecretId = "secretSetByAppConfig";

            private IClientApiKeyBuilder builder;
            private IConfigurationManager config;

            [TestInitialize]
            public void Setup()
            {
                this.config = Substitute.For<IConfigurationManager>();
                this.config.AppSettings.Returns(new System.Collections.Specialized.NameValueCollection()
                {
                    { idVariableName, fakeApiKeyId },
                    { secretVariableName, fakeApiSecretId }
                });

                this.builder = new DefaultClientApiKeyBuilder(config, Substitute.For<IEnvironment>(), Substitute.For<IFile>());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file_from_app_config()
            {
                var clientApiKey = this.builder.Build();

                clientApiKey.GetId().ShouldBe(fakeApiKeyId);
                clientApiKey.GetSecret().ShouldBe(fakeApiSecretId);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void App_config_values_are_lower_priority_than_explicit()
            {
                var clientApiKey = this.builder
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                clientApiKey.GetId().ShouldBe("different");
                clientApiKey.GetSecret().ShouldBe("also_different");
            }
        }

        [TestClass]
        public class Building_With_Stream
        {
            private IClientApiKeyBuilder builder;

            [TestInitialize]
            public void Setup()
            {
                this.builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), Substitute.For<IEnvironment>(), Substitute.For<IFile>());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_values_from_stream()
            {
                var streamContents =
                    "apiKey.id = streams_r_neet\r\n" +
                    "apiKey.secret = pls_dont_steal!\r\n";

                using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(streamContents)))
                {
                    var clientApiKey = this.builder
                        .SetInputStream(stream)
                        .Build();

                    clientApiKey.GetId().ShouldBe("streams_r_neet");
                    clientApiKey.GetSecret().ShouldBe("pls_dont_steal!");
                }
            }
        }

        [TestClass]
        public class Building_With_Explicit_Values
        {
            private IClientApiKeyBuilder builder;

            [TestInitialize]
            public void Setup()
            {
                this.builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), Substitute.For<IEnvironment>(), Substitute.For<IFile>());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_explicit_values()
            {
                var clientApiKey = this.builder
                    .SetId("foo")
                    .SetSecret("bar")
                    .Build();

                clientApiKey.GetId().ShouldBe("foo");
                clientApiKey.GetSecret().ShouldBe("bar");
            }
        }

        [TestClass]
        public class Building_With_Properties_File
        {
            private readonly string testLocation = "test.properties";
            private readonly string fileContents =
                "apiKey.id = foobar\r\n" +
                "apiKey.secret = bazsecret!\r\n";

            private IClientApiKeyBuilder builder;
            private IFile file;

            [TestInitialize]
            public void Setup()
            {
                this.file = Substitute.For<IFile>();
                this.file.ReadAllText(testLocation)
                    .Returns(fileContents);

                this.builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), Substitute.For<IEnvironment>(), file);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file()
            {
                var clientApiKey = this.builder
                    .SetFileLocation(testLocation)
                    .Build();

                clientApiKey.GetId().ShouldBe("foobar");
                clientApiKey.GetSecret().ShouldBe("bazsecret!");
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = this.builder
                    .SetFileLocation(testLocation)
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                clientApiKey.GetId().ShouldBe("different");
                clientApiKey.GetSecret().ShouldBe("also_different");
            }
        }
    }
}
