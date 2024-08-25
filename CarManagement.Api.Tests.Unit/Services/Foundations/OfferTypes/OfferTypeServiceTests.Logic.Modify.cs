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
        public async Task ShouldModifyOfferTypeAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            OfferType randomOfferType = CreateRandomModifyOfferType(randomDate);
            OfferType inputOfferType = randomOfferType;
            OfferType storageOfferType = inputOfferType.DeepClone();
            storageOfferType.UpdatedDate = randomOfferType.CreatedDate;
            OfferType updatedOfferType = inputOfferType;
            OfferType expectedOfferType = updatedOfferType.DeepClone();
            Guid offerTypeId = inputOfferType.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(offerTypeId))
                    .ReturnsAsync(storageOfferType);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateOfferTypeAsync(inputOfferType))
                    .ReturnsAsync(updatedOfferType);

            // when
            OfferType actualOfferType =
               await this.offerTypeService.ModifyOfferTypeAsync(inputOfferType);

            // then
            actualOfferType.Should().BeEquivalentTo(expectedOfferType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(offerTypeId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateOfferTypeAsync(inputOfferType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
