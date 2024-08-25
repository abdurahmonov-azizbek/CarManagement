// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;
using CarManagement.Api.Models.CarModels.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarModels
{
    public partial class CarModelServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCarModelIsNullAndLogItAsync()
        {
            // given
            CarModel nullCarModel = null;
            var nullCarModelException = new NullCarModelException();

            var expectedCarModelValidationException =
                new CarModelValidationException(nullCarModelException);

            // when
            ValueTask<CarModel> modifyCarModelTask = this.carModelService.ModifyCarModelAsync(nullCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(
                    modifyCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfCarModelIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            CarModel invalidCarModel = new CarModel
            {
                Name = invalidString
            };

            var invalidCarModelException = new InvalidCarModelException();

				invalidCarModelException.AddData(
					key: nameof(CarModel.Id),
					values: "Id is required");

				invalidCarModelException.AddData(
					key: nameof(CarModel.Name),
					values: "Text is required");



            invalidCarModelException.AddData(
                key: nameof(CarModel.CreatedDate),
                values: "Date is required");

            invalidCarModelException.AddData(
                key: nameof(CarModel.UpdatedDate),
                values: new[]
                    {
                        "Date is required",
                        "Date is not recent",
                        $"Date is the same as {nameof(CarModel.CreatedDate)}"
                    }
                );

            var expectedCarModelValidationException =
                new CarModelValidationException(invalidCarModelException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<CarModel> modifyCarModelTask = this.carModelService.ModifyCarModelAsync(invalidCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(
                    modifyCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            CarModel randomCarModel = CreateRandomCarModel(randomDateTime);
            CarModel invalidCarModel = randomCarModel;
            var invalidCarModelException = new InvalidCarModelException();

            invalidCarModelException.AddData(
                key: nameof(CarModel.UpdatedDate),
                values: $"Date is the same as {nameof(CarModel.CreatedDate)}");

            var expectedCarModelValidationException =
                new CarModelValidationException(invalidCarModelException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(invalidCarModel);

            CarModelValidationException actualCarModelValidationException =
                 await Assert.ThrowsAsync<CarModelValidationException>(
                    modifyCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(invalidCarModel.Id), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            CarModel randomCarModel = CreateRandomCarModel(dateTime);
            CarModel inputCarModel = randomCarModel;
            inputCarModel.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidCarModelException = new InvalidCarModelException();

            invalidCarModelException.AddData(
                key: nameof(CarModel.UpdatedDate),
                values: "Date is not recent");

            var expectedCarModelValidatonException =
                new CarModelValidationException(invalidCarModelException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(inputCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(
                    modifyCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidatonException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCarModelDoesNotExistAndLogItAsync()
        {
            // given
            int negativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            CarModel randomCarModel = CreateRandomCarModel(dateTime);
            CarModel nonExistCarModel = randomCarModel;
            nonExistCarModel.CreatedDate = dateTime.AddMinutes(negativeMinutes);
            CarModel nullCarModel = null;

            var notFoundCarModelException = new NotFoundCarModelException(nonExistCarModel.Id);

            var expectedCarModelValidationException =
                new CarModelValidationException(notFoundCarModelException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(nonExistCarModel.Id))
                    .ReturnsAsync(nullCarModel);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(nonExistCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(
                    modifyCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(nonExistCarModel.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            CarModel randomCarModel = CreateRandomModifyCarModel(randomDateTime);
            CarModel invalidCarModel = randomCarModel.DeepClone();
            CarModel storageCarModel = invalidCarModel.DeepClone();
            storageCarModel.CreatedDate = storageCarModel.CreatedDate.AddMinutes(randomMinutes);
            storageCarModel.UpdatedDate = storageCarModel.UpdatedDate.AddMinutes(randomMinutes);
            var invalidCarModelException = new InvalidCarModelException();
            Guid carModelId = invalidCarModel.Id;

            invalidCarModelException.AddData(
                key: nameof(CarModel.CreatedDate),
                values: $"Date is not same as {nameof(CarModel.CreatedDate)}");

            var expectedCarModelValidationException =
                new CarModelValidationException(invalidCarModelException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(carModelId)).ReturnsAsync(storageCarModel);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(invalidCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(modifyCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(invalidCarModel.Id), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            CarModel randomCarModel = CreateRandomModifyCarModel(randomDateTime);
            CarModel invalidCarModel = randomCarModel;
            CarModel storageCarModel = randomCarModel.DeepClone();
            invalidCarModel.UpdatedDate = storageCarModel.UpdatedDate;
            Guid carModelId = invalidCarModel.Id;
            var invalidCarModelException = new InvalidCarModelException();

            invalidCarModelException.AddData(
                key: nameof(CarModel.UpdatedDate),
                values: $"Date is the same as {nameof(CarModel.UpdatedDate)}");

            var expectedCarModelValidationException =
                new CarModelValidationException(invalidCarModelException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(invalidCarModel.Id)).ReturnsAsync(storageCarModel);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(invalidCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(modifyCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(carModelId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
