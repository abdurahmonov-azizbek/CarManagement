// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
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
        public void ShouldRetrieveAllOffers()
        {
            //given
            IQueryable<Offer> randomOffers = CreateRandomOffers();
            IQueryable<Offer> storageOffers = randomOffers;
            IQueryable<Offer> expectedOffers = storageOffers.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOffers()).Returns(storageOffers);

            //when
            IQueryable<Offer> actualOffers =
                this.offerService.RetrieveAllOffers();

            //then
            actualOffers.Should().BeEquivalentTo(expectedOffers);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOffers(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
