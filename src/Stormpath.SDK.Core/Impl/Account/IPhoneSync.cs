using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal interface IPhoneSync :
        IDeletableSync,
        ISaveableSync<IPhone>
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IPhone.GetAccountAsync(System.Threading.CancellationToken)"/>
        /// </summary>
        /// <returns>The account.</returns>
        IAccount GetAccount();
    }
}
