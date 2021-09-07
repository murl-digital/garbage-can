using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Models
{
    public class QueuedBooster
    {
        [SwaggerSchema("The XP multiplier")]
        public float Multiplier { get; set; }
        [SwaggerSchema("This booster's duration, in seconds. This is converted into a UTC timestamp when activated")]
        public long DurationInSeconds { get; set; }
    }
}
