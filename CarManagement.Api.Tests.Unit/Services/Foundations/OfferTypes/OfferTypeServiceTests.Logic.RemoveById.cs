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
        public async Task ShouldRemoveOfferTypeByIdAsync()
        {
            // given
            Guid randomOfferTypeId = Guid.NewGuid();
            Guid inputOfferTypeId = randomOfferTypeId;
            OfferType randomOfferType = CreateRandomOfferType();
            OfferType storageOfferType = randomOfferType;
            OfferType expectedInputOfferType = storageOfferType;
            OfferType deletedOfferType = expectedInputOfferType;
            OfferType expectedOfferType = deletedOfferType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(inputOfferTypeId))
                    .ReturnsAsync(storageOfferType);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteOfferTypeAsync(expectedInputOfferType))
                    .ReturnsAsync(deletedOfferType);

            // when
            OfferType actualOfferType = await this.offerTypeService
                .RemoveOfferTypeByIdAsync(inputOfferTypeId);

            // then
            actualOfferType.Should().BeEquivalentTo(expectedOfferType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(inputOfferTypeId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteOfferTypeAsync(expectedInputOfferType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
