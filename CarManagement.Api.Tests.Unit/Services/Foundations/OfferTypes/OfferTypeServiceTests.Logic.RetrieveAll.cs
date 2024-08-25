// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
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
        public void ShouldRetrieveAllOfferTypes()
        {
            //given
            IQueryable<OfferType> randomOfferTypes = CreateRandomOfferTypes();
            IQueryable<OfferType> storageOfferTypes = randomOfferTypes;
            IQueryable<OfferType> expectedOfferTypes = storageOfferTypes.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOfferTypes()).Returns(storageOfferTypes);

            //when
            IQueryable<OfferType> actualOfferTypes =
                this.offerTypeService.RetrieveAllOfferTypes();

            //then
            actualOfferTypes.Should().BeEquivalentTo(expectedOfferTypes);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOfferTypes(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
