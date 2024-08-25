// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.OfferTypes
{
    public partial class OfferTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidOfferTypeId = Guid.Empty;

            var invalidOfferTypeException = new InvalidOfferTypeException();

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.Id),
                values: "Id is required");

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(invalidOfferTypeException);

            // when
            ValueTask<OfferType> removeOfferTypeByIdTask =
                this.offerTypeService.RemoveOfferTypeByIdAsync(invalidOfferTypeId);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(
                    removeOfferTypeByIdTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteOfferTypeAsync(It.IsAny<OfferType>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfOfferTypeIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomOfferTypeId = Guid.NewGuid();
            Guid inputOfferTypeId = randomOfferTypeId;
            OfferType noOfferType = null;

            var notFoundOfferTypeException =
                new NotFoundOfferTypeException(inputOfferTypeId);

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(notFoundOfferTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(inputOfferTypeId)).ReturnsAsync(noOfferType);

            // when
            ValueTask<OfferType> removeOfferTypeByIdTask =
                this.offerTypeService.RemoveOfferTypeByIdAsync(inputOfferTypeId);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(
                    removeOfferTypeByIdTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
