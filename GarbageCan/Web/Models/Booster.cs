using System;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Models
{
    public class Booster
    {
        [SwaggerSchema("The XP multiplier. Internally 1.0 is added to the total (e.g. 0.5 -> 1.5)")]
        public float Multiplier { get; set; }
        [SwaggerSchema("The date and time this booster will expire at, in UTC")]
        public DateTime Expiration { get; set; }
    }
}