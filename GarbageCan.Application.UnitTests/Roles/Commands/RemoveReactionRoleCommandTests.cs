﻿using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.ReactionRoles.Commands.RemoveReactionRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class RemoveReactionRoleCommandTests
    {
        private ApplicationFixture _appFixture;
        private IApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
            };
        }

        [Test]
        public async Task ShouldRemoveReactionRole_WhenReactionRolesExists()
        {
            const int id = 5;

            ReactionRole role = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.ReactionRoles, new ReactionRole {Id = id});
            mockDbSet.When(x => x.Remove(Arg.Any<ReactionRole>())).Do(x => role = x.Arg<ReactionRole>());

            await _appFixture.SendAsync(new RemoveReactionRoleCommand
            {
                Id = id
            });

            await _dbContext.Received(1).SaveChangesAsync(default);
            _dbContext.ReactionRoles.Received(1).Remove(Arg.Any<ReactionRole>());

            role.Should().NotBeNull();
            role.Id.Should().Be(id);
        }

        [Test]
        public void ShouldThrowNotFoundException_WhenReactionRolesDoesNotExist()
        {
            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles);

            var command = new RemoveReactionRoleCommand
            {
                Id = 50
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<NotFoundException>()
                .WithMessage("Couldn't find Reaction Role");
        }

        [Theory]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void ShouldThrowValidationException_WhenIdIsInvalid(int id)
        {
            var command = new RemoveReactionRoleCommand
            {
                Id = id
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }
    }
}
