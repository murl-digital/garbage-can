namespace GarbageCan.Application.Members.Queries
{
    public class MemberVm
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public double Xp { get; set; }
        public int Level { get; set; }
        public bool IsBot { get; set; }
    }
}
