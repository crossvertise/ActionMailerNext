
namespace ActionMailerNext.Interfaces
{
    public interface IMailResponse
    {
        string Email { get; }

        DeliveryStatus DeliveryStatus { get; }
    }
}
