using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Api
{
    public interface IApiKey
    {
        Task<string> GetIdAsync();

        Task<string> GetSecretAsync();

        // ApiKeyStatus

        //void SetStatus()

        //Task<IAccount> 

        //Task<ITenant>

        // void Save()
    }
}
