using System.Configuration;

namespace Stormpath.Demo
{
    internal static class Config
    {
        public static string ApiKeyFileLocation
        {
            get { return ConfigurationManager.AppSettings["ApiKeyFileLocation"]; }
        }
    }
}
