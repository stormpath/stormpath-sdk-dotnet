using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on the <see cref="IPhone">Phone</see> resource.
    /// </summary>
    public static class SyncPhoneExtensions
    {
        /// <summary>
        /// Synchronously gets the <see cref="IAccount">Account</see> associated with this phone.
        /// </summary>
        /// <param name="phone">The phone.</param>
        /// <returns>The account.</returns>
        public static IAccount GetAccount(this IPhone phone)
            => (phone as IPhoneSync).GetAccount();
    }
}
