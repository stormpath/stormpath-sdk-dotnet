using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultChallenge : AbstractInstanceResource, IChallenge
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

        public Task SubmitAsync(string code, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
