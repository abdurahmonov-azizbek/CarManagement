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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalidOfferTypeId = Guid.Empty;
            var invalidOfferTypeException = new InvalidOfferTypeException();

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.Id),
                values: "Id is required");

            var excpectedOfferTypeValidationException = new
                OfferTypeValidationException(invalidOfferTypeException);

            //when
            ValueTask<OfferType> retrieveOfferTypeByIdTask =
                this.offerTypeService.RetrieveOfferTypeByIdAsync(invalidOfferTypeId);

            OfferTypeValidationException actuallOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(
                    retrieveOfferTypeByIdTask.AsTask);

            //then
            actuallOfferTypeValidationException.Should()
                .BeEquivalentTo(excpectedOfferTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfOfferTypeIsNotFoundAndLogItAsync()
        {
            Guid someOfferTypeId = Guid.NewGuid();
            OfferType noOfferType = null;

            var notFoundOfferTypeException =
                new NotFoundOfferTypeException(someOfferTypeId);

            var excpectedOfferTypeValidationException =
                new OfferTypeValidationException(notFoundOfferTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noOfferType);

            //when 
            ValueTask<OfferType> retrieveOfferTypeByIdTask =
                this.offerTypeService.RetrieveOfferTypeByIdAsync(someOfferTypeId);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(
                    retrieveOfferTypeByIdTask.AsTask);

            //then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(excpectedOfferTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
