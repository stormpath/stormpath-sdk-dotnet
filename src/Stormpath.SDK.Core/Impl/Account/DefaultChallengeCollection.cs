using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultChallengeCollection : CollectionResourceQueryable<IChallenge>, IChallengeCollection
    {
        public DefaultChallengeCollection(string collectionHref, IInternalDataStore dataStore)
            : base(collectionHref, dataStore)
        {
        }

        public Task<IChallenge> AddAsync(CancellationToken cancellationToken)
            => AddAsync(new ChallengeCreationOptions(), cancellationToken);

        public Task<IChallenge> AddAsync(ChallengeCreationOptions options, CancellationToken cancellationToken)
        {
            return (_dataStore as IInternalAsyncDataStore).CreateAsync<ChallengeCreationOptions, IChallenge>(
                _collectionHref,
                options,
                null,
                cancellationToken);
        }
    }
}
