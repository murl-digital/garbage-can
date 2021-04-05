using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Models
{
    public class Member
    {
        [SwaggerSchema("The member's ID, in Discord's snowflake format")]
        public ulong Id { get; set; }
        [SwaggerSchema("The member's display name")]
        public string Name { get; set; }
        [SwaggerSchema("The member's XP, formatted as a double with 2 digits of precision")]
        public double Xp { get; set; }
        [SwaggerSchema("The member's level")]
        public int Level { get; set; }
    }
}