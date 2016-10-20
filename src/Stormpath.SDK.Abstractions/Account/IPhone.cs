using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// A phone number stored in Stormpath.
    /// </summary>
    public interface IPhone : 
        IResource,
        IDeletable,
        ISaveable<IPhone>,
        IAuditable
    {
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        string Number { get; set; }

        /// <summary>
        /// Gets or sets the phone's name, if any.
        /// </summary>
        /// <value>The phone's name, or <see langword="null"/>.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone's description, if any.
        /// </summary>
        /// <value>The phone's description, or <see langword="null"/>.</value>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the phone's verification status.
        /// </summary>
        /// <value>The phone's verification status.</value>
        PhoneVerificationStatus VerificationStatus { get; set; }

        /// <summary>
        /// Gets or sets the phone's status.
        /// </summary>
        /// <value>The phone's verification status.</value>
        PhoneStatus Status { get; set; }

        /// <summary>
        /// Gets the <see cref="IAccount">Account</see> associated with this phone.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The account.</returns>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
