using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.XP.Data.Entities
{
	[Table("xp_users")]
	public class EntityUser
	{
		[Key] [DatabaseGenerated(DatabaseGeneratedOption.None)] public ulong id { get; set; }
		public int lvl { get; set; }

		private double _xp;
		public double xp
		{
			get => _xp;
			set => _xp = Math.Round(value, 1);
		}
	}
}