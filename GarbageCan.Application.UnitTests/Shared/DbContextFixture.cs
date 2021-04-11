using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.XP;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.Shared
{
    public class DbContextFixture
    {
        public DbContextFixture()
        {
            MockContext = new Mock<IApplicationDbContext>();

            var setDbSet = XPUsers.AsQueryable().BuildMockDbSet();

            setDbSet.Setup(x => x.FindAsync(It.IsAny<Guid>())).Returns(() => new ValueTask<EntityUser>(Task.FromResult(XPUsers.First())));

            MockContext.Setup(x => x.XPUsers).Returns(() => setDbSet.Object);
        }

        public Mock<IApplicationDbContext> MockContext { get; }

        public List<EntityUser> XPUsers { get; set; } = new List<EntityUser>();
    }
}