// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;
using CarManagement.Api.Models.CarTypes.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarTypes
{
    public partial class CarTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCarTypeIsNullAndLogItAsync()
        {
            // given
            CarType nullCarType = null;
            var nullCarTypeException = new NullCarTypeException();

            var expectedCarTypeValidationException =
                new CarTypeValidationException(nullCarTypeException);

            // when
            ValueTask<CarType> modifyCarTypeTask = this.carTypeService.ModifyCarTypeAsync(nullCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(
                    modifyCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfCarTypeIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            CarType invalidCarType = new CarType
            {
                Name = invalidString
            };

            var invalidCarTypeException = new InvalidCarTypeException();

				invalidCarTypeException.AddData(
					key: nameof(CarType.Id),
					values: "Id is required");

				invalidCarTypeException.AddData(
					key: nameof(CarType.Name),
					values: "Text is required");



            invalidCarTypeException.AddData(
                key: nameof(CarType.CreatedDate),
                values: "Date is required");

            invalidCarTypeException.AddData(
                key: nameof(CarType.UpdatedDate),
                values: new[]
                    {
                        "Date is required",
                        "Date is not recent",
                        $"Date is the same as {nameof(CarType.CreatedDate)}"
                    }
                );

            var expectedCarTypeValidationException =
                new CarTypeValidationException(invalidCarTypeException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<CarType> modifyCarTypeTask = this.carTypeService.ModifyCarTypeAsync(invalidCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(
                    modifyCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            CarType randomCarType = CreateRandomCarType(randomDateTime);
            CarType invalidCarType = randomCarType;
            var invalidCarTypeException = new InvalidCarTypeException();

            invalidCarTypeException.AddData(
                key: nameof(CarType.UpdatedDate),
                values: $"Date is the same as {nameof(CarType.CreatedDate)}");

            var expectedCarTypeValidationException =
                new CarTypeValidationException(invalidCarTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(invalidCarType);

            CarTypeValidationException actualCarTypeValidationException =
                 await Assert.ThrowsAsync<CarTypeValidationException>(
                    modifyCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(invalidCarType.Id), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

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
            CarType randomCarType = CreateRandomCarType(dateTime);
            CarType inputCarType = randomCarType;
            inputCarType.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidCarTypeException = new InvalidCarTypeException();

            invalidCarTypeException.AddData(
                key: nameof(CarType.UpdatedDate),
                values: "Date is not recent");

            var expectedCarTypeValidatonException =
                new CarTypeValidationException(invalidCarTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(inputCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(
                    modifyCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidatonException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCarTypeDoesNotExistAndLogItAsync()
        {
            // given
            int negativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            CarType randomCarType = CreateRandomCarType(dateTime);
            CarType nonExistCarType = randomCarType;
            nonExistCarType.CreatedDate = dateTime.AddMinutes(negativeMinutes);
            CarType nullCarType = null;

            var notFoundCarTypeException = new NotFoundCarTypeException(nonExistCarType.Id);

            var expectedCarTypeValidationException =
                new CarTypeValidationException(notFoundCarTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(nonExistCarType.Id))
                    .ReturnsAsync(nullCarType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(nonExistCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(
                    modifyCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(nonExistCarType.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

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
            CarType randomCarType = CreateRandomModifyCarType(randomDateTime);
            CarType invalidCarType = randomCarType.DeepClone();
            CarType storageCarType = invalidCarType.DeepClone();
            storageCarType.CreatedDate = storageCarType.CreatedDate.AddMinutes(randomMinutes);
            storageCarType.UpdatedDate = storageCarType.UpdatedDate.AddMinutes(randomMinutes);
            var invalidCarTypeException = new InvalidCarTypeException();
            Guid carTypeId = invalidCarType.Id;

            invalidCarTypeException.AddData(
                key: nameof(CarType.CreatedDate),
                values: $"Date is not same as {nameof(CarType.CreatedDate)}");

            var expectedCarTypeValidationException =
                new CarTypeValidationException(invalidCarTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(carTypeId)).ReturnsAsync(storageCarType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(invalidCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(modifyCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(invalidCarType.Id), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

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
            CarType randomCarType = CreateRandomModifyCarType(randomDateTime);
            CarType invalidCarType = randomCarType;
            CarType storageCarType = randomCarType.DeepClone();
            invalidCarType.UpdatedDate = storageCarType.UpdatedDate;
            Guid carTypeId = invalidCarType.Id;
            var invalidCarTypeException = new InvalidCarTypeException();

            invalidCarTypeException.AddData(
                key: nameof(CarType.UpdatedDate),
                values: $"Date is the same as {nameof(CarType.UpdatedDate)}");

            var expectedCarTypeValidationException =
                new CarTypeValidationException(invalidCarTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(invalidCarType.Id)).ReturnsAsync(storageCarType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(invalidCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(modifyCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(carTypeId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
