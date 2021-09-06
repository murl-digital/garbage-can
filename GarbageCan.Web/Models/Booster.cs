using System;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Models
{
    public class Booster
    {
        [SwaggerSchema("The XP multiplier")]
        public float Multiplier { get; set; }
        [SwaggerSchema("The date and time this booster will expire at, in UTC")]
        public DateTime Expiration { get; set; }
    }
}
