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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidCarModelId = Guid.Empty;

            var invalidCarModelException = new InvalidCarModelException();

            invalidCarModelException.AddData(
                key: nameof(CarModel.Id),
                values: "Id is required");

            var expectedCarModelValidationException =
                new CarModelValidationException(invalidCarModelException);

            // when
            ValueTask<CarModel> removeCarModelByIdTask =
                this.carModelService.RemoveCarModelByIdAsync(invalidCarModelId);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(
                    removeCarModelByIdTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCarModelAsync(It.IsAny<CarModel>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfCarModelIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomCarModelId = Guid.NewGuid();
            Guid inputCarModelId = randomCarModelId;
            CarModel noCarModel = null;

            var notFoundCarModelException =
                new NotFoundCarModelException(inputCarModelId);

            var expectedCarModelValidationException =
                new CarModelValidationException(notFoundCarModelException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(inputCarModelId)).ReturnsAsync(noCarModel);

            // when
            ValueTask<CarModel> removeCarModelByIdTask =
                this.carModelService.RemoveCarModelByIdAsync(inputCarModelId);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(
                    removeCarModelByIdTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
