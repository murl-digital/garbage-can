using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities
{
	[Table("xp_available_slots")]
	public class EntityAvailableSlot
	{
		[Key] public int id { get; set; }
		public ulong channel_id { get; set; }
	}
}