using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace Stormpath.SDK.Redis
{
    internal static class Constants
    {
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.DateTimeOffset,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            TypeNameHandling = TypeNameHandling.All
        };
    }
}