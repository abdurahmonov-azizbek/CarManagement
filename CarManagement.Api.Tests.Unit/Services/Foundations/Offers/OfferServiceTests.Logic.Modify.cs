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
        public async Task ShouldModifyOfferAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Offer randomOffer = CreateRandomModifyOffer(randomDate);
            Offer inputOffer = randomOffer;
            Offer storageOffer = inputOffer.DeepClone();
            storageOffer.UpdatedDate = randomOffer.CreatedDate;
            Offer updatedOffer = inputOffer;
            Offer expectedOffer = updatedOffer.DeepClone();
            Guid offerId = inputOffer.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferByIdAsync(offerId))
                    .ReturnsAsync(storageOffer);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateOfferAsync(inputOffer))
                    .ReturnsAsync(updatedOffer);

            // when
            Offer actualOffer =
               await this.offerService.ModifyOfferAsync(inputOffer);

            // then
            actualOffer.Should().BeEquivalentTo(expectedOffer);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferByIdAsync(offerId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateOfferAsync(inputOffer), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
