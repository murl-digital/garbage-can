using GarbageCan.Domain.Enums;

namespace GarbageCan.Domain.Entities.Presence
{
    public class CustomStatus
    {
        public int Id { get; set; }
        public Activity Activity { get; set; }
        public string Name { get; set; }
    }
}
