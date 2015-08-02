using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Stormpath.SDK.Api;

namespace Stormpath.SDK.Tests.SDK
{
    [TestClass]
    public class ClientApiKeysBuilderTests
    {
        public static void DeleteFileIfExists(string location)
        {
            if (File.Exists(location))
                File.Delete(location);
        }

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
            // Note that these tests cannot be run in parallel as they attempt to edit the same file.

            private readonly string defaultLocation =
                System.IO.Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), ".stormpath\\", "apiKey.properties");
            private readonly string fileContents =
                "apiKey.id = 144JVZINOF5EBNCMG9EXAMPLE\r\n" +
                "apiKey.secret = lWxOiKqKPNwJmSldbiSkEbkNjgh2uRSNAb+AEXAMPLE";

            [TestInitialize]
            public void Setup()
            {
                if (File.Exists(defaultLocation))
                    throw new Exception("You already have a file in the default Stormpath key location.");

                if (!Directory.Exists(Path.GetDirectoryName(defaultLocation)))
                    Directory.CreateDirectory(Path.GetDirectoryName(defaultLocation));

                File.WriteAllText(defaultLocation, fileContents);
            }

            [TestCleanup]
            public void Teardown()
            {
                DeleteFileIfExists(defaultLocation);
                bool isDirectoryEmpty = !Directory.GetFiles(Path.GetDirectoryName(defaultLocation)).Any();
                if (isDirectoryEmpty)
                    Directory.Delete(Path.GetDirectoryName(defaultLocation));
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_default_properties_file()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .Build();

                Assert.AreEqual("144JVZINOF5EBNCMG9EXAMPLE", clientApiKey.GetId());
                Assert.AreEqual("lWxOiKqKPNwJmSldbiSkEbkNjgh2uRSNAb+AEXAMPLE", clientApiKey.GetSecret());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Default_properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                Assert.AreEqual("different", clientApiKey.GetId());
                Assert.AreEqual("also_different", clientApiKey.GetSecret());
            }
        }

        [TestClass]
        public class Building_With_Environment_Variable_File
        {
            // Note that these tests cannot be run in parallel because they edit the same environment variable.

            private readonly string envVariableName = "STORMPATH_API_KEY_FILE";
            private readonly string testLocation = "envfile.properties";
            private readonly string fileContents =
                "apiKey.id = envId\r\n" +
                "apiKey.secret = envSecret\r\n";

            [TestInitialize]
            public void Setup()
            {
                bool environmentClean = string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envVariableName));
                if (!environmentClean)
                    throw new Exception($"You already have a value for the environment variable {envVariableName}.");

                DeleteFileIfExists(testLocation);

                File.WriteAllText(testLocation, fileContents);
                Environment.SetEnvironmentVariable(envVariableName, testLocation);
            }

            [TestCleanup]
            public void Teardown()
            {
                if (Environment.GetEnvironmentVariable(envVariableName) == testLocation)
                    Environment.SetEnvironmentVariable(envVariableName, null);

                DeleteFileIfExists(testLocation);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file_from_env_variable()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .Build();

                Assert.AreEqual("envId", clientApiKey.GetId());
                Assert.AreEqual("envSecret", clientApiKey.GetSecret());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Env_variable_properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                Assert.AreEqual("different", clientApiKey.GetId());
                Assert.AreEqual("also_different", clientApiKey.GetSecret());
            }
        }

        [TestClass]
        public class Building_With_Environment_Variable_Values
        {
            // Note that these tests cannot be run in parallel because they edit the same environment variable.

            private readonly string idVariableName = "STORMPATH_API_KEY_ID";
            private readonly string fakeApiKeyId = "idSetByEnv";
            private readonly string secretVariableName = "STORMPATH_API_KEY_SECRET";
            private readonly string fakeApiSecretId = "secretSetByEnv";

            [TestInitialize]
            public void Setup()
            {
                bool environmentClean =
                    string.IsNullOrEmpty(Environment.GetEnvironmentVariable(idVariableName)) &&
                    string.IsNullOrEmpty(Environment.GetEnvironmentVariable(secretVariableName));
                if (!environmentClean)
                    throw new Exception($"You already have a value for the environment variable {idVariableName} or {secretVariableName}.");

                Environment.SetEnvironmentVariable(idVariableName, fakeApiKeyId);
                Environment.SetEnvironmentVariable(secretVariableName, fakeApiSecretId);
            }

            [TestCleanup]
            public void Teardown()
            {
                if (Environment.GetEnvironmentVariable(idVariableName) == fakeApiKeyId)
                    Environment.SetEnvironmentVariable(idVariableName, null);
                if (Environment.GetEnvironmentVariable(secretVariableName) == fakeApiSecretId)
                    Environment.SetEnvironmentVariable(secretVariableName, null);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_values_from_env_variables()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .Build();

                Assert.AreEqual(fakeApiKeyId, clientApiKey.GetId());
                Assert.AreEqual(fakeApiSecretId, clientApiKey.GetSecret());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Env_variable_values_is_lower_priority_than_explicit()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                Assert.AreEqual("different", clientApiKey.GetId());
                Assert.AreEqual("also_different", clientApiKey.GetSecret());
            }
        }

        [TestClass]
        public class Building_With_Explicit_Values
        {
            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_explicit_values()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .SetId("foo")
                    .SetSecret("bar")
                    .Build();

                Assert.AreEqual("foo", clientApiKey.GetId());
                Assert.AreEqual("bar", clientApiKey.GetSecret());
            }
        }

        [TestClass]
        public class Building_With_Properties_File
        {
            private readonly string testLocation = "test.properties";
            private readonly string fileContents =
                "apiKey.id = foobar\r\n" +
                "apiKey.secret = bazsecret!\r\n";

            [TestInitialize]
            public void Setup()
            {
                DeleteFileIfExists(testLocation);
                File.WriteAllText(testLocation, fileContents);
            }

            [TestCleanup]
            public void Teardown()
            {
                DeleteFileIfExists(testLocation);
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void With_properties_file()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .SetFileLocation(testLocation)
                    .Build();

                Assert.AreEqual("foobar", clientApiKey.GetId());
                Assert.AreEqual("bazsecret!", clientApiKey.GetSecret());
            }

            [TestMethod]
            [TestCategory(nameof(ClientApiKeys) + ".Builder")]
            public void Properties_file_is_lower_priority_than_explicit()
            {
                var clientApiKey = ClientApiKeys.Builder()
                    .SetFileLocation(testLocation)
                    .SetId("different")
                    .SetSecret("also_different")
                    .Build();

                Assert.AreEqual("different", clientApiKey.GetId());
                Assert.AreEqual("also_different", clientApiKey.GetSecret());
            }
        }
    }
}
