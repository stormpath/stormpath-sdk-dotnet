using System;
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

        public Task<IChallenge> AddAsync(ChallengeCreationOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
