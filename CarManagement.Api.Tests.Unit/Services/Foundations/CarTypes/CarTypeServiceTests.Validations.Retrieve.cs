// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;
using CarManagement.Api.Models.CarTypes.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarTypes
{
    public partial class CarTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalidCarTypeId = Guid.Empty;
            var invalidCarTypeException = new InvalidCarTypeException();

            invalidCarTypeException.AddData(
                key: nameof(CarType.Id),
                values: "Id is required");

            var excpectedCarTypeValidationException = new
                CarTypeValidationException(invalidCarTypeException);

            //when
            ValueTask<CarType> retrieveCarTypeByIdTask =
                this.carTypeService.RetrieveCarTypeByIdAsync(invalidCarTypeId);

            CarTypeValidationException actuallCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(
                    retrieveCarTypeByIdTask.AsTask);

            //then
            actuallCarTypeValidationException.Should()
                .BeEquivalentTo(excpectedCarTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfCarTypeIsNotFoundAndLogItAsync()
        {
            Guid someCarTypeId = Guid.NewGuid();
            CarType noCarType = null;

            var notFoundCarTypeException =
                new NotFoundCarTypeException(someCarTypeId);

            var excpectedCarTypeValidationException =
                new CarTypeValidationException(notFoundCarTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noCarType);

            //when 
            ValueTask<CarType> retrieveCarTypeByIdTask =
                this.carTypeService.RetrieveCarTypeByIdAsync(someCarTypeId);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(
                    retrieveCarTypeByIdTask.AsTask);

            //then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(excpectedCarTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
