using System;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Models
{
    public class QueuedBooster
    {
        [SwaggerSchema("The XP multiplier. Internally 1.0 is added to the total (e.g. 0.5 -> 1.5)")]
        public float Multiplier { get; set; }
        [SwaggerSchema("This booster's duration, in seconds. This is converted into a UTC timestamp when activated")]
        public long DurationInSeconds { get; set; }
    }
}