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
        public async Task ShouldAddPenaltyAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Penalty randomPenalty = CreateRandomPenalty(randomDate);
            Penalty inputPenalty = randomPenalty;
            Penalty persistedPenalty = inputPenalty;
            Penalty expectedPenalty = persistedPenalty.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPenaltyAsync(inputPenalty)).ReturnsAsync(persistedPenalty);

            // when
            Penalty actualPenalty = await this.penaltyService
                .AddPenaltyAsync(inputPenalty);

            // then
            actualPenalty.Should().BeEquivalentTo(expectedPenalty);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPenaltyAsync(inputPenalty), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}