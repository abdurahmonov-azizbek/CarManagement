// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;
using CarManagement.Api.Models.CarModels.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarModels
{
    public partial class CarModelServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalidCarModelId = Guid.Empty;
            var invalidCarModelException = new InvalidCarModelException();

            invalidCarModelException.AddData(
                key: nameof(CarModel.Id),
                values: "Id is required");

            var excpectedCarModelValidationException = new
                CarModelValidationException(invalidCarModelException);

            //when
            ValueTask<CarModel> retrieveCarModelByIdTask =
                this.carModelService.RetrieveCarModelByIdAsync(invalidCarModelId);

            CarModelValidationException actuallCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(
                    retrieveCarModelByIdTask.AsTask);

            //then
            actuallCarModelValidationException.Should()
                .BeEquivalentTo(excpectedCarModelValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfCarModelIsNotFoundAndLogItAsync()
        {
            Guid someCarModelId = Guid.NewGuid();
            CarModel noCarModel = null;

            var notFoundCarModelException =
                new NotFoundCarModelException(someCarModelId);

            var excpectedCarModelValidationException =
                new CarModelValidationException(notFoundCarModelException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noCarModel);

            //when 
            ValueTask<CarModel> retrieveCarModelByIdTask =
                this.carModelService.RetrieveCarModelByIdAsync(someCarModelId);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(
                    retrieveCarModelByIdTask.AsTask);

            //then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(excpectedCarModelValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
