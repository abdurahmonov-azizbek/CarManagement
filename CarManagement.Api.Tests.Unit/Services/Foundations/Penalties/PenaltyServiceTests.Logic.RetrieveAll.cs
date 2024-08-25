// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
using CarManagement.Api.Models.Penalties;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Penalties
{
    public partial class PenaltyServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllPenalties()
        {
            //given
            IQueryable<Penalty> randomPenalties = CreateRandomPenalties();
            IQueryable<Penalty> storagePenalties = randomPenalties;
            IQueryable<Penalty> expectedPenalties = storagePenalties.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPenalties()).Returns(storagePenalties);

            //when
            IQueryable<Penalty> actualPenalties =
                this.penaltyService.RetrieveAllPenalties();

            //then
            actualPenalties.Should().BeEquivalentTo(expectedPenalties);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPenalties(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
