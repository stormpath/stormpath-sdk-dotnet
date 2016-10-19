using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed partial class DefaultChallenge
    {
        public IAccount GetAccount()
            => GetInternalSyncDataStore().GetResource<IAccount>(GetLinkProperty(AccountPropertyName).Href);

        public IFactor GetFactor()
            => GetInternalSyncDataStore().GetResource<IFactor>(
                GetLinkProperty(FactorPropertyName).Href);

        public IChallenge Submit(string code)
            => GetInternalSyncDataStore().Create<ChallengeSubmitData, IChallenge>(
                InternalHref,
                new ChallengeSubmitData
                {
                    Code = code
                });

        public bool Validate(string code)
        {
            Submit(code);
            return Status == ChallengeStatus.Success;
        }
    }
}
