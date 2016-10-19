namespace Stormpath.SDK.Account
{
    public sealed class PhoneCreationOptions
    {
        public string Number { get; set; }

        public PhoneVerificationStatus VerificationStatus { get; set; }

        public PhoneStatus Status { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
