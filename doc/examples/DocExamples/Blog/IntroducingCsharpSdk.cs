using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK.Application;

namespace Stormpath.SDK.DocExamples.Blogs
{
    /// <summary>
    /// Code listings for https://stormpath.com/blog/introducing-csharp-sdk
    /// </summary>
    public class IntroducingCsharpSdk
    {
        private async Task LinqExample()
        {
            IApplication myApp = null;

            #region code

            var newUsersThisMonthNamedSkywalker = await myApp.GetAccounts()
                .Where(x => x.Surname == "Skywalker" && x.CreatedAt.Within(2015, 09))
                .OrderBy(x => x.Email)
                .Take(10)
                .ToListAsync();

            #endregion
        }
    }
}
