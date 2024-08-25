// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Offers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Offers
{
    public partial class OfferServiceTests
    {
        [Fact]
        public async Task ShouldAddOfferAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Offer randomOffer = CreateRandomOffer(randomDate);
            Offer inputOffer = randomOffer;
            Offer persistedOffer = inputOffer;
            Offer expectedOffer = persistedOffer.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertOfferAsync(inputOffer)).ReturnsAsync(persistedOffer);

            // when
            Offer actualOffer = await this.offerService
                .AddOfferAsync(inputOffer);

            // then
            actualOffer.Should().BeEquivalentTo(expectedOffer);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertOfferAsync(inputOffer), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}