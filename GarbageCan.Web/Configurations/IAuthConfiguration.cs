namespace GarbageCan.Web.Configurations
{
    public interface IAuthConfiguration
    {
        string Authority { get; }
        string Audience { get; }
        bool Enabled { get; }
    }
}
