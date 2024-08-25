// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
using CarManagement.Api.Models.Users;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllUsers()
        {
            //given
            IQueryable<User> randomUsers = CreateRandomUsers();
            IQueryable<User> storageUsers = randomUsers;
            IQueryable<User> expectedUsers = storageUsers.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUsers()).Returns(storageUsers);

            //when
            IQueryable<User> actualUsers =
                this.userService.RetrieveAllUsers();

            //then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUsers(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
