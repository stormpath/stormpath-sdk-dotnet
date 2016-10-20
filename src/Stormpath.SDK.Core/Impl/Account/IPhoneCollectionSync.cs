using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    internal interface IPhoneCollectionSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IPhoneCollection.AddAsync(string, System.Threading.CancellationToken)"/>
        /// </summary>
        /// <param name="number">The phone number.</param>
        /// <returns>The new <see cref="IPhone">Phone</see>.</returns>
        IPhone Add(string number);

        /// <summary>
        /// Synchronous counterpart to <see cref="IPhoneCollection.AddAsync(PhoneCreationOptions, System.Threading.CancellationToken)"/>
        /// </summary>
        /// <param name="options">The phone creation options.</param>
        /// <returns>The new <see cref="IPhone">Phone</see>.</returns>
        IPhone Add(PhoneCreationOptions options);
    }
}
