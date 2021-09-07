using System.Collections.Generic;

namespace GarbageCan.Application.XP.Queries.GetTopUsersByXP
{
    public class GetTopUsersByXPQueryVm
    {
        public List<GetTopUsersByXPQueryVmUser> TopTenUsers { get; set; }
        public GetTopUsersByXPQueryVmUser ContextUser { get; set; }
    }
}