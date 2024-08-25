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
        public async Task ShouldRetrieveOfferTypeByIdAsync()
        {
            //given
            Guid randomOfferTypeId = Guid.NewGuid();
            Guid inputOfferTypeId = randomOfferTypeId;
            OfferType randomOfferType = CreateRandomOfferType();
            OfferType storageOfferType = randomOfferType;
            OfferType excpectedOfferType = randomOfferType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(inputOfferTypeId)).ReturnsAsync(storageOfferType);

            //when
            OfferType actuallOfferType = await this.offerTypeService.RetrieveOfferTypeByIdAsync(inputOfferTypeId);

            //then
            actuallOfferType.Should().BeEquivalentTo(excpectedOfferType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(inputOfferTypeId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}