using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents a collection of <see cref="IPhone">Phones</see>.
    /// </summary>
    public interface IPhoneCollection : IAsyncQueryable<IPhone>
    {
        /// <summary>
        /// Adds a new phone.
        /// </summary>
        /// <param name="number">The phone number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IPhone">Phone</see>.</returns>
        Task<IPhone> AddAsync(string number, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new phone with the specified options.
        /// </summary>
        /// <param name="options">The phone creation options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IPhone">Phone</see>.</returns>
        Task<IPhone> AddAsync(PhoneCreationOptions options, CancellationToken cancellationToken = default(CancellationToken));
    }
}
