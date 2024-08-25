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
        public async Task ShouldRetrieveOfferByIdAsync()
        {
            //given
            Guid randomOfferId = Guid.NewGuid();
            Guid inputOfferId = randomOfferId;
            Offer randomOffer = CreateRandomOffer();
            Offer storageOffer = randomOffer;
            Offer excpectedOffer = randomOffer.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferByIdAsync(inputOfferId)).ReturnsAsync(storageOffer);

            //when
            Offer actuallOffer = await this.offerService.RetrieveOfferByIdAsync(inputOfferId);

            //then
            actuallOffer.Should().BeEquivalentTo(excpectedOffer);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferByIdAsync(inputOfferId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}