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
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            CarType nullCarType = null;
            var nullCarTypeException = new NullCarTypeException();

            var expectedCarTypeValidationException =
                new CarTypeValidationException(nullCarTypeException);

            // when
            ValueTask<CarType> addCarTypeTask = this.carTypeService.AddCarTypeAsync(nullCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(addCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedCarTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarTypeAsync(It.IsAny<CarType>()), Times.Never);

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
            CarType invalidCarType = new CarType()
            {
                Name = invalidText
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
					values: "Date is required");



            var expectedCarTypeValidationException =
                new CarTypeValidationException(invalidCarTypeException);

            // when
            ValueTask<CarType> addCarTypeTask = this.carTypeService.AddCarTypeAsync(invalidCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(addCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarTypeAsync(It.IsAny<CarType>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            CarType randomCarType = CreateRandomCarType(randomDate);
            CarType invalidCarType = randomCarType;
            invalidCarType.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidCarTypeException = new InvalidCarTypeException();

            invalidCarTypeException.AddData(
                key: nameof(CarType.CreatedDate),
                values: $"Date is not same as {nameof(CarType.UpdatedDate)}");

            var expectedCarTypeValidationException = new CarTypeValidationException(invalidCarTypeException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<CarType> addCarTypeTask = this.carTypeService.AddCarTypeAsync(invalidCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(addCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should().BeEquivalentTo(expectedCarTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarTypeAsync(It.IsAny<CarType>()), Times.Never);

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
            CarType randomCarType = CreateRandomCarType(invalidDateTime);
            CarType invalidCarType = randomCarType;
            var invalidCarTypeException = new InvalidCarTypeException();

            invalidCarTypeException.AddData(
                key: nameof(CarType.CreatedDate),
                values: "Date is not recent");

            var expectedCarTypeValidationException =
                new CarTypeValidationException(invalidCarTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<CarType> addCarTypeTask = this.carTypeService.AddCarTypeAsync(invalidCarType);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(addCarTypeTask.AsTask);

            // then
            actualCarTypeValidationException.Should().
                BeEquivalentTo(expectedCarTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarTypeAsync(It.IsAny<CarType>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}