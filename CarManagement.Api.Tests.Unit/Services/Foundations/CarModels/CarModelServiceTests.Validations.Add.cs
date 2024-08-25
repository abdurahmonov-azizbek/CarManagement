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
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            CarModel nullCarModel = null;
            var nullCarModelException = new NullCarModelException();

            var expectedCarModelValidationException =
                new CarModelValidationException(nullCarModelException);

            // when
            ValueTask<CarModel> addCarModelTask = this.carModelService.AddCarModelAsync(nullCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(addCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedCarModelValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarModelAsync(It.IsAny<CarModel>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfJobIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            CarModel invalidCarModel = new CarModel()
            {
                Name = invalidText
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
					values: "Date is required");



            var expectedCarModelValidationException =
                new CarModelValidationException(invalidCarModelException);

            // when
            ValueTask<CarModel> addCarModelTask = this.carModelService.AddCarModelAsync(invalidCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(addCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should()
                .BeEquivalentTo(expectedCarModelValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarModelAsync(It.IsAny<CarModel>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            CarModel randomCarModel = CreateRandomCarModel(randomDate);
            CarModel invalidCarModel = randomCarModel;
            invalidCarModel.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidCarModelException = new InvalidCarModelException();

            invalidCarModelException.AddData(
                key: nameof(CarModel.CreatedDate),
                values: $"Date is not same as {nameof(CarModel.UpdatedDate)}");

            var expectedCarModelValidationException = new CarModelValidationException(invalidCarModelException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<CarModel> addCarModelTask = this.carModelService.AddCarModelAsync(invalidCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(addCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should().BeEquivalentTo(expectedCarModelValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarModelAsync(It.IsAny<CarModel>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinutes))]
        public async Task ShouldThrowValidationExceptionIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidMinutes)
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            DateTimeOffset invalidDateTime = randomDate.AddMinutes(invalidMinutes);
            CarModel randomCarModel = CreateRandomCarModel(invalidDateTime);
            CarModel invalidCarModel = randomCarModel;
            var invalidCarModelException = new InvalidCarModelException();

            invalidCarModelException.AddData(
                key: nameof(CarModel.CreatedDate),
                values: "Date is not recent");

            var expectedCarModelValidationException =
                new CarModelValidationException(invalidCarModelException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<CarModel> addCarModelTask = this.carModelService.AddCarModelAsync(invalidCarModel);

            CarModelValidationException actualCarModelValidationException =
                await Assert.ThrowsAsync<CarModelValidationException>(addCarModelTask.AsTask);

            // then
            actualCarModelValidationException.Should().
                BeEquivalentTo(expectedCarModelValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedCarModelValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarModelAsync(It.IsAny<CarModel>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}