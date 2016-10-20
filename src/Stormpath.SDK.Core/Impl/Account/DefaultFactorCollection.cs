using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultFactorCollection : CollectionResourceQueryable<IFactor>, IFactorCollection, IFactorCollectionSync
    {
        public DefaultFactorCollection(string collectionHref, IInternalDataStore dataStore)
            : base(collectionHref, dataStore)
        {
        }

        public Task<ISmsFactor> AddAsync(SmsFactorCreationOptions options, CancellationToken cancellationToken)
            => (_dataStore as IInternalAsyncDataStore).CreateAsync<SmsFactorCreationData, ISmsFactor>(
                _collectionHref,
                GetCreationData(options),
                GetCreationOptions(options),
                null,
                cancellationToken);

        public ISmsFactor Add(SmsFactorCreationOptions options)
            => (_dataStore as IInternalSyncDataStore).Create<SmsFactorCreationData, ISmsFactor>(
                _collectionHref,
                GetCreationData(options),
                GetCreationOptions(options),
                null);

        public Task<IGoogleAuthenticatorFactor> AddAsync(GoogleAuthenticatorFactorCreationOptions options, CancellationToken cancellationToken)
            => (_dataStore as IInternalAsyncDataStore).CreateAsync<GoogleAuthenticatorFactorCreationData, IGoogleAuthenticatorFactor>(
                    _collectionHref,
                    GetCreationData(options),
                    GetCreationOptions(options),
                    null,
                    cancellationToken);

        public IGoogleAuthenticatorFactor Add(GoogleAuthenticatorFactorCreationOptions options)
            => (_dataStore as IInternalSyncDataStore).Create<GoogleAuthenticatorFactorCreationData, IGoogleAuthenticatorFactor>(
                    _collectionHref,
                    GetCreationData(options),
                    GetCreationOptions(options),
                    null);

        private static SmsFactorCreationData GetCreationData(SmsFactorCreationOptions options) => 
            new SmsFactorCreationData
        {
            Phone = new SmsFactorCreationPhoneData
            {
                Number = options.Number
            },
            Status = options.Status
        };

        private static DefaultCreationOptions GetCreationOptions(SmsFactorCreationOptions options)
        {
            var parameters = new Dictionary<string, string>();
            if (options.Challenge)
            {
                parameters.Add("challenge", "true");
            }

            return new DefaultCreationOptions(parameters);
        }

        private static GoogleAuthenticatorFactorCreationData GetCreationData(GoogleAuthenticatorFactorCreationOptions options) => 
            new GoogleAuthenticatorFactorCreationData
        {
            AccountName = options.AccountName,
            Issuer = options.Issuer,
            Status = options.Status
        };

        private static DefaultCreationOptions GetCreationOptions(GoogleAuthenticatorFactorCreationOptions options)
        {
            var parameters = new Dictionary<string, string>();
            if (options.Challenge)
            {
                parameters.Add("challenge", "true");
            }

            return new DefaultCreationOptions(parameters);
        }
    }
}
