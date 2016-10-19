using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on the <see cref="ISmsFactor">SMS Factor</see> resource.
    /// </summary>
    public static class SyncSmsFactorExtensions
    {
        /// <summary>
        /// Synchronously gets the <see cref="IPhone">Phone</see> associated with this factor.
        /// </summary>
        /// <param name="smsFactor">The factor.</param>
        /// <returns>The <see cref="IPhone">Phone</see> associated with this factor.</returns>
        public static IPhone GetPhone(this ISmsFactor smsFactor)
            => (smsFactor as ISmsFactorSync).GetPhone();
    }
}
