using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Api
{
    public interface IClientApiKey
    {
        string GetId();

        string GetSecret();
    }
}
