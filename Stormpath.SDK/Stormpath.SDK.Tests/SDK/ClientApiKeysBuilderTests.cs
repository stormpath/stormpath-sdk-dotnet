using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Api;
using Stormpath.SDK.Impl.Utility;
using NSubstitute;
using Shouldly;

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

            private IClientApiKeyBuilder _builder;
            private IFile _file;

            [TestInitialize]
            public void Setup()
            {
                _file = Substitute.For<IFile>();
                _file.ReadAllText(defaultLocation)
                    .Returns(fileContents);

                _builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), Substitute.For<IEnvironment>(), _file);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_default_properties_file()
            {
                var clientApiKey = _builder.Build();

                clientApiKey.GetId().ShouldBe("144JVZINOF5EBNCMG9EXAMPLE");
                clientApiKey.GetSecret().ShouldBe("lWxOiKqKPNwJmSldbiSkEbkNjgh2uRSNAb+AEXAMPLE");
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Default_properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = _builder
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

            private IClientApiKeyBuilder _builder;
            private IEnvironment _environment;
            private IFile _file;

            [TestInitialize]
            public void Setup()
            {
                _environment = Substitute.For<IEnvironment>();
                _environment.GetEnvironmentVariable(envVariableName)
                    .Returns(testLocation);

                _file = Substitute.For<IFile>();
                _file.ReadAllText(testLocation)
                    .Returns(fileContents);

                _builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), _environment, _file);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file_from_env_variable()
            {
                var clientApiKey = _builder.Build();

                clientApiKey.GetId().ShouldBe("envId");
                clientApiKey.GetSecret().ShouldBe("envSecret");
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Env_variable_properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = _builder
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

            private IClientApiKeyBuilder _builder;
            private IEnvironment _environment;

            [TestInitialize]
            public void Setup()
            {
                _environment = Substitute.For<IEnvironment>();
                _environment.GetEnvironmentVariable(idVariableName)
                    .Returns(fakeApiKeyId);
                _environment.GetEnvironmentVariable(secretVariableName)
                    .Returns(fakeApiSecretId);

                _builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), _environment, Substitute.For<IFile>());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_values_from_env_variables()
            {
                var clientApiKey = _builder
                    .Build();

                clientApiKey.GetId().ShouldBe(fakeApiKeyId);
                clientApiKey.GetSecret().ShouldBe(fakeApiSecretId);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Env_variable_values_are_lower_priority_than_explicit()
            {
                var clientApiKey = _builder
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

            private IClientApiKeyBuilder _builder;
            private IConfigurationManager _config;
            private IFile _file;

            [TestInitialize]
            public void Setup()
            {
                _config = Substitute.For<IConfigurationManager>();
                _config.AppSettings.Returns(new System.Collections.Specialized.NameValueCollection()
                {
                    { configVariableName, testLocation }
                });

                _file = Substitute.For<IFile>();
                _file.ReadAllText(testLocation)
                    .Returns(fileContents);

                _builder = new DefaultClientApiKeyBuilder(_config, Substitute.For<IEnvironment>(), _file);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file_from_app_config()
            {
                var clientApiKey = _builder.Build();

                clientApiKey.GetId().ShouldBe("fromAppConfig?");
                clientApiKey.GetSecret().ShouldBe("fooSecret!");
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void App_config_properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = _builder
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

            private IClientApiKeyBuilder _builder;
            private IConfigurationManager _config;

            [TestInitialize]
            public void Setup()
            {
                _config = Substitute.For<IConfigurationManager>();
                _config.AppSettings.Returns(new System.Collections.Specialized.NameValueCollection()
                {
                    { idVariableName, fakeApiKeyId},
                    { secretVariableName, fakeApiSecretId }
                });

                _builder = new DefaultClientApiKeyBuilder(_config, Substitute.For<IEnvironment>(), Substitute.For<IFile>());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file_from_app_config()
            {
                var clientApiKey = _builder.Build();

                clientApiKey.GetId().ShouldBe(fakeApiKeyId);
                clientApiKey.GetSecret().ShouldBe(fakeApiSecretId);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void App_config_values_are_lower_priority_than_explicit()
            {
                var clientApiKey = _builder
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
            private IClientApiKeyBuilder _builder;

            [TestInitialize]
            public void Setup()
            {
                _builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), Substitute.For<IEnvironment>(), Substitute.For<IFile>());
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
                    var clientApiKey = _builder
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
            private IClientApiKeyBuilder _builder;

            [TestInitialize]
            public void Setup()
            {
                _builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), Substitute.For<IEnvironment>(), Substitute.For<IFile>());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_explicit_values()
            {
                var clientApiKey = _builder
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

            private IClientApiKeyBuilder _builder;
            private IFile _file;

            [TestInitialize]
            public void Setup()
            {
                _file = Substitute.For<IFile>();
                _file.ReadAllText(testLocation)
                    .Returns(fileContents);

                _builder = new DefaultClientApiKeyBuilder(Substitute.For<IConfigurationManager>(), Substitute.For<IEnvironment>(), _file);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file()
            {
                var clientApiKey = _builder
                    .SetFileLocation(testLocation)
                    .Build();

                clientApiKey.GetId().ShouldBe("foobar");
                clientApiKey.GetSecret().ShouldBe("bazsecret!");
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = _builder
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
