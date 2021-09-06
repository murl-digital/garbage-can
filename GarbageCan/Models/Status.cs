using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Models
{
    public class Status
    {
        [SwaggerSchema("The bot's gateway ping")]
        public int Ping { get; init; }
    }
}
