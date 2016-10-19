using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on the <see cref="ISmsFactor">SMS Factor</see> resource.
    /// </summary>
    internal interface ISmsFactorSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="ISmsFactor.GetPhoneAsync(System.Threading.CancellationToken)"/>
        /// </summary>
        /// <returns>The <see cref="IPhone">Phone</see> associated with this factor.</returns>
        IPhone GetPhone();
    }
}
