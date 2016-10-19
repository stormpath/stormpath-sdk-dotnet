using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on the <see cref="IPhoneCollection">Phone collection</see>.
    /// </summary>
    public static class SyncPhoneCollectionExtensions
    {
        /// <summary>
        /// Synchronously adds a new phone.
        /// </summary>
        /// <param name="phoneCollection">The <see cref="IPhoneCollection"/>.</param>
        /// <param name="number">The phone number.</param>
        /// <returns>The new <see cref="IPhone">Phone</see>.</returns>
        public static IPhone Add(this IPhoneCollection phoneCollection, string number)
            => (phoneCollection as IPhoneCollectionSync).Add(number);

        /// <summary>
        /// Synchronously adds a new phone with the specified options.
        /// </summary>
        /// <param name="phoneCollection">The <see cref="IPhoneCollection"/>.</param>
        /// <param name="options">The phone creation options.</param>
        /// <returns>The new <see cref="IPhone">Phone</see>.</returns>
        public static IPhone Add(
            this IPhoneCollection phoneCollection,
            PhoneCreationOptions options)
            => (phoneCollection as IPhoneCollectionSync).Add(options);
    }
}
