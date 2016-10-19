using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed partial class DefaultChallenge : AbstractInstanceResource, IChallenge, IChallengeSync
    {
        private const string StatusPropertyName = "status";
        private const string MessagePropertyName = "message";
        private const string AccountPropertyName = "account";
        private const string FactorPropertyName = "factor";

        public DefaultChallenge(ResourceData data)
            : base(data)
        {
        }

        public ChallengeStatus Status 
            => GetEnumProperty<ChallengeStatus>(StatusPropertyName);

        public string Message
            => GetStringProperty(MessagePropertyName);


        public Task<IAccount> GetAccountAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().GetResourceAsync<IAccount>(
                GetLinkProperty(AccountPropertyName).Href,
                cancellationToken);

        public Task<IFactor> GetFactorAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().GetResourceAsync<IFactor>(
                GetLinkProperty(FactorPropertyName).Href,
                cancellationToken);

        public Task<IChallenge> SubmitAsync(string code, CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().CreateAsync<ChallengeSubmitData, IChallenge>(
                InternalHref,
                new ChallengeSubmitData
                {
                    Code = code
                },
                cancellationToken);

        public async Task<bool> ValidateAsync(string code, CancellationToken cancellationToken)
        {
            await SubmitAsync(code, cancellationToken);
            return Status == ChallengeStatus.Success;
        }

        public Task<bool> DeleteAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        // Sync methods are implemented in DefaultChallenge.Sync.cs
    }
}
