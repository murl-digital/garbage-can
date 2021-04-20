using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using GarbageCan.Domain.Entities.XP;
using MockQueryable.NSubstitute;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace GarbageCan.Application.UnitTests.Shared
{
    public class DbContextFixture
    {
        public DbContextFixture()
        {
            MockContext = Substitute.For<IApplicationDbContext>();

            MockContext.XPUsers.ReturnsForAnyArgs(_ => XPUsers.AsQueryable().BuildMockDbSet());
            MockContext.ReactionRoles.ReturnsForAnyArgs(_ => ReactionRoles.AsQueryable().BuildMockDbSet());
        }

        public IApplicationDbContext MockContext { get; }

        public List<ReactionRole> ReactionRoles { get; set; } = new List<ReactionRole>();
        public List<User> XPUsers { get; set; } = new List<User>();
    }
}