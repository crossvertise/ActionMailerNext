
namespace ActionMailerNext.Interfaces
{
    using ActionMailerNext.Implementations.SMTP;

    public interface IMailResponse
    {
        string Email { get; }

        DeliveryStatus DeliveryStatus { get; }
    }
}
