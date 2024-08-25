// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.OfferTypes
{
    public partial class OfferTypeServiceTests
    {
        [Fact]
        public async Task ShouldAddOfferTypeAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            OfferType randomOfferType = CreateRandomOfferType(randomDate);
            OfferType inputOfferType = randomOfferType;
            OfferType persistedOfferType = inputOfferType;
            OfferType expectedOfferType = persistedOfferType.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertOfferTypeAsync(inputOfferType)).ReturnsAsync(persistedOfferType);

            // when
            OfferType actualOfferType = await this.offerTypeService
                .AddOfferTypeAsync(inputOfferType);

            // then
            actualOfferType.Should().BeEquivalentTo(expectedOfferType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertOfferTypeAsync(inputOfferType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}