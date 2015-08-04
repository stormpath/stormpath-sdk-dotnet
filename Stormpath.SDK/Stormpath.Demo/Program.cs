using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Api;

namespace Stormpath.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiKey = ClientApiKeys.Builder()
                .SetId("myID")
                .SetSecret("secret")
                .Build();

            
        }
    }
}
