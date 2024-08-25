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
        public async Task ShouldModifyPenaltyAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Penalty randomPenalty = CreateRandomModifyPenalty(randomDate);
            Penalty inputPenalty = randomPenalty;
            Penalty storagePenalty = inputPenalty.DeepClone();
            storagePenalty.UpdatedDate = randomPenalty.CreatedDate;
            Penalty updatedPenalty = inputPenalty;
            Penalty expectedPenalty = updatedPenalty.DeepClone();
            Guid penaltyId = inputPenalty.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPenaltyByIdAsync(penaltyId))
                    .ReturnsAsync(storagePenalty);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdatePenaltyAsync(inputPenalty))
                    .ReturnsAsync(updatedPenalty);

            // when
            Penalty actualPenalty =
               await this.penaltyService.ModifyPenaltyAsync(inputPenalty);

            // then
            actualPenalty.Should().BeEquivalentTo(expectedPenalty);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPenaltyByIdAsync(penaltyId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePenaltyAsync(inputPenalty), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
