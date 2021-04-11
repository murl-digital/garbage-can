using GarbageCan.Domain.Entities.XP;

namespace GarbageCan.Application.XP.Queries.GetTopUsersByXP
{
    public class GetTopUsersByXPQueryVmUser
    {
        public GetTopUsersByXPQueryVmUser(EntityUser u, int index)
        {
            User = u;
            Place = index + 1;
        }

        public int Place { get; set; }
        public string DisplayName { get; set; }
        public EntityUser User { get; set; }
    }
}