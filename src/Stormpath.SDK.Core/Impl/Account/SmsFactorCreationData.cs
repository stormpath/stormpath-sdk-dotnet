namespace Stormpath.SDK.Impl.Account
{
    internal sealed class SmsFactorCreationData
    {
        public string Type { get; set; } = "SMS";

        public SmsFactorCreationPhoneData Phone { get; set; }

        public string Status { get; set; }
    }

    internal sealed class SmsFactorCreationPhoneData
    {
        public string Number { get; set; }
    }
}
