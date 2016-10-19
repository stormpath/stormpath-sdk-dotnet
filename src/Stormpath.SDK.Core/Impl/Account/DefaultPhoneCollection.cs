using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultPhoneCollection : CollectionResourceQueryable<IPhone>, IPhoneCollection //, IPhoneCollectionSync TODO
    {
        public DefaultPhoneCollection(string collectionHref, IInternalDataStore dataStore)
            : base(collectionHref, dataStore)
        {
        }

        public Task<IPhone> AddAsync(string number, CancellationToken cancellationToken)
            => (_dataStore as IInternalAsyncDataStore).CreateAsync<PhoneCreationOptions, IPhone>(
                _collectionHref,
                new PhoneCreationOptions
                {
                    Number = number
                },
                cancellationToken);

        public Task<IPhone> AddAsync(PhoneCreationOptions options, CancellationToken cancellationToken)
            => (_dataStore as IInternalAsyncDataStore).CreateAsync<PhoneCreationOptions, IPhone>(
                _collectionHref,
                options,
                cancellationToken);
    }
}
