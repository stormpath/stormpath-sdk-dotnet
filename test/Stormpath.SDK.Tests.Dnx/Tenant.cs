using System.Threading.Tasks;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Tests.Dnx
{
    public class Tenant
    {
        [Fact]
        public async Task Getting_current_tenant()
        {
            var client = Clients.Builder()
                .SetConnectionTimeout(30)
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .Build();

            var tenant = await client.GetCurrentTenantAsync();

            Assert.NotNull(tenant);
        }
    }
}
