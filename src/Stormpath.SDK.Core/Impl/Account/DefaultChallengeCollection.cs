using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultChallengeCollection : CollectionResourceQueryable<IChallenge>, IChallengeCollection, IChallengeCollectionSync
    {
        public DefaultChallengeCollection(string collectionHref, IInternalDataStore dataStore)
            : base(collectionHref, dataStore)
        {
        }

        public Task<IChallenge> AddAsync(CancellationToken cancellationToken)
            => AddAsync(new ChallengeCreationOptions(), cancellationToken);

        public IChallenge Add()
            => Add(new ChallengeCreationOptions());

        public Task<IChallenge> AddAsync(ChallengeCreationOptions options, CancellationToken cancellationToken)
            => (_dataStore as IInternalAsyncDataStore).CreateAsync<ChallengeCreationOptions, IChallenge>(
                _collectionHref,
                options,
                null,
                cancellationToken);

        public IChallenge Add(ChallengeCreationOptions options)
            => (_dataStore as IInternalSyncDataStore).Create<ChallengeCreationOptions, IChallenge>(
                _collectionHref,
                options,
                null);
    }
}
