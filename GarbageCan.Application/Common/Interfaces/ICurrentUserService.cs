namespace GarbageCan.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string DisplayName { get; }
    }
}