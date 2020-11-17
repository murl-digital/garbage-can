using System;

namespace GarbageCan.XP.Data.Models
{
	public class ActiveBooster
	{
		public AvailableSlot slot { get; set; }
		public DateTime expirationDate { get; set; }
		public float multipler { get; set; }
	}
}