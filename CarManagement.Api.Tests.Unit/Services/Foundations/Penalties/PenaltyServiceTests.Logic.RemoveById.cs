// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldRemovePenaltyByIdAsync()
        {
            // given
            Guid randomPenaltyId = Guid.NewGuid();
            Guid inputPenaltyId = randomPenaltyId;
            Penalty randomPenalty = CreateRandomPenalty();
            Penalty storagePenalty = randomPenalty;
            Penalty expectedInputPenalty = storagePenalty;
            Penalty deletedPenalty = expectedInputPenalty;
            Penalty expectedPenalty = deletedPenalty.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPenaltyByIdAsync(inputPenaltyId))
                    .ReturnsAsync(storagePenalty);

            this.storageBrokerMock.Setup(broker =>
                broker.DeletePenaltyAsync(expectedInputPenalty))
                    .ReturnsAsync(deletedPenalty);

            // when
            Penalty actualPenalty = await this.penaltyService
                .RemovePenaltyByIdAsync(inputPenaltyId);

            // then
            actualPenalty.Should().BeEquivalentTo(expectedPenalty);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPenaltyByIdAsync(inputPenaltyId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePenaltyAsync(expectedInputPenalty), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
