using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class SmsFactorCreationData
    {
        public string Type { get; } = "SMS";

        public SmsFactorCreationPhoneData Phone { get; set; }

        public FactorStatus Status { get; set; }
    }

    internal sealed class SmsFactorCreationPhoneData
    {
        public string Number { get; set; }
    }
}
