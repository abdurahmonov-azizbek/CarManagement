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
        public async Task ShouldRemoveOfferByIdAsync()
        {
            // given
            Guid randomOfferId = Guid.NewGuid();
            Guid inputOfferId = randomOfferId;
            Offer randomOffer = CreateRandomOffer();
            Offer storageOffer = randomOffer;
            Offer expectedInputOffer = storageOffer;
            Offer deletedOffer = expectedInputOffer;
            Offer expectedOffer = deletedOffer.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferByIdAsync(inputOfferId))
                    .ReturnsAsync(storageOffer);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteOfferAsync(expectedInputOffer))
                    .ReturnsAsync(deletedOffer);

            // when
            Offer actualOffer = await this.offerService
                .RemoveOfferByIdAsync(inputOfferId);

            // then
            actualOffer.Should().BeEquivalentTo(expectedOffer);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferByIdAsync(inputOfferId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteOfferAsync(expectedInputOffer), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
