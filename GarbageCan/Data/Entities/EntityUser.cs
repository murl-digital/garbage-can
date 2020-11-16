using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities
{
	[Table("xp_users")]
	public class EntityUser
	{
		[Key] [MaxLength(18)] public string id { get; set; }
		public int lvl { get; set; }
		public double xp { get; set; }
	}
}