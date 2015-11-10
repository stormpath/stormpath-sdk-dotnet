using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Stormpath.SDK.Extensions.Cache.Redis
{
    public class RedisCacheProvider
    {
        IConnectionMultiplexer redisConnection;

        public RedisCacheProvider(string redisConfiguration)
        {
            this.redisConnection = ConnectionMultiplexer.Connect(redisConfiguration);
        }
    }
}
