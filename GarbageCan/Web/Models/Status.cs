using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Models
{
    public class Status
    {
        [SwaggerSchema("The bot's gateway ping")]
        public int Ping { get; init; }
    }
}