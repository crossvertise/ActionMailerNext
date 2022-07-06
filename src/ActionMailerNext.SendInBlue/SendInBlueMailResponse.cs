namespace ActionMailerNext.SendInBlue
{
    using ActionMailerNext.Interfaces;

    public class SendInBlueMailResponse : IMailResponse
    {
        public string Email { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }
    }
}
