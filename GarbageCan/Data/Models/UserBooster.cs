namespace GarbageCan.Data.Models
{
    public class UserBooster
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public float multiplier { get; set; }
        public long duration_in_seconds { get; set; }
    }
}