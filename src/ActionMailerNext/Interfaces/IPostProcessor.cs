namespace ActionMailerNext.Interfaces
{
    public interface IPostProcessor
    {
        MailAttributes Execute(MailAttributes mailAttributes);
    }
}
