using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// A SMS <see cref="IFactor">Factor</see>.
    /// </summary>
    public interface ISmsFactor : IFactor
    {
        /// <summary>
        /// Gets the <see cref="IPhone">Phone</see> associated with this factor.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="IPhone">Phone</see> associated with this factor.</returns>
        Task<IPhone> GetPhoneAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
