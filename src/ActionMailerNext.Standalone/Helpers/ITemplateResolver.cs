namespace ActionMailerNext.Standalone.Helpers
{
    public interface ITemplateResolver
    {
        string Resolve(string name);
    }
}