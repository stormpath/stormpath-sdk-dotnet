using System;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.UAT.Net461
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestRestSharpAdapterCreation();

            Console.WriteLine("All net461 UATs done");
        }

        private static void TestRestSharpAdapterCreation()
        {
            var client = Clients.Builder()
                .SetHttpClient(HttpClients.Create().RestSharpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetApiKeyId("fake")
                .SetApiKeySecret("reallyfake")
                .Build();
        }
    }
}
