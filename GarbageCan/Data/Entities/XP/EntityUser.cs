using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.XP
{
    [Table("xp_users")]
    public class EntityUser
    {
        private double _xp;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong id { get; set; }

        public int lvl { get; set; }

        public double xp
        {
            get => _xp;
            set => _xp = Math.Round(value, 1);
        }
    }
}