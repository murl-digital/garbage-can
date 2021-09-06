namespace GarbageCan.Web.Configurations
{
    public class AuthConfiguration : IAuthConfiguration
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public bool Enabled { get; set; }
    }
}
