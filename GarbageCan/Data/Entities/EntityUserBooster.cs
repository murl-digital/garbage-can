using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities
{
	[Table("xp_user_boosters")]
	public class EntityUserBooster
	{
		[Key] public string id { get; set; }
		public ulong user_id { get; set; }
		public float multiplier { get; set; }
		public long duration_in_seconds { get; set; }
	}
}