using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultPhone : AbstractInstanceResource, IPhone
    {
        private const string NumberPropertyName = "number";
        private const string DescriptionPropertyName = "description";
        private const string NamePropertyName = "name";
        private const string VerificationStatusPropertyName = "verificationStatus";
        private const string StatusPropertyName = "status";
        private const string AccountPropertyName = "account";

        public DefaultPhone(ResourceData data)
            : base(data)
        {
        }

        public string Number
        {
            get { return GetStringProperty(NumberPropertyName); }
            set { SetProperty(NumberPropertyName, value); }
        }

        public string Name
        {
            get { return GetStringProperty(NamePropertyName); }
            set { SetProperty(NamePropertyName, value); }
        }

        public string Description
        {
            get { return GetStringProperty(DescriptionPropertyName); }
            set { SetProperty(DescriptionPropertyName, value); }
        }

        public PhoneStatus Status
        {
            get { return GetEnumProperty<PhoneStatus>(StatusPropertyName); }
            set { SetProperty(StatusPropertyName, value); }
        }

        public PhoneVerificationStatus VerificationStatus
            => GetEnumProperty<PhoneVerificationStatus>(VerificationStatusPropertyName);

        public Task<IAccount> GetAccountAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().GetResourceAsync<IAccount>(
                GetLinkProperty(AccountPropertyName).Href,
                cancellationToken);

        public Task<IPhone> SaveAsync(CancellationToken cancellationToken)
            => SaveAsync<IPhone>(cancellationToken);

        public Task<bool> DeleteAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);
    }
}
