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
        public async Task ShouldRetrievePenaltyByIdAsync()
        {
            //given
            Guid randomPenaltyId = Guid.NewGuid();
            Guid inputPenaltyId = randomPenaltyId;
            Penalty randomPenalty = CreateRandomPenalty();
            Penalty storagePenalty = randomPenalty;
            Penalty excpectedPenalty = randomPenalty.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPenaltyByIdAsync(inputPenaltyId)).ReturnsAsync(storagePenalty);

            //when
            Penalty actuallPenalty = await this.penaltyService.RetrievePenaltyByIdAsync(inputPenaltyId);

            //then
            actuallPenalty.Should().BeEquivalentTo(excpectedPenalty);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPenaltyByIdAsync(inputPenaltyId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}