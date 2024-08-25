// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Cars;
using CarManagement.Api.Models.Cars.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Cars
{
    public partial class CarServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            Car randomCar = CreateRandomCar();
            Car someCar = randomCar;
            Guid carId = someCar.Id;
            SqlException sqlException = CreateSqlException();

            var failedCarStorageException =
                new FailedCarStorageException(sqlException);

            var expectedCarDependencyException =
                new CarDependencyException(failedCarStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<Car> modifyCarTask =
                this.carService.ModifyCarAsync(someCar);

            CarDependencyException actualCarDependencyException =
                await Assert.ThrowsAsync<CarDependencyException>(
                    modifyCarTask.AsTask);

            // then
            actualCarDependencyException.Should().BeEquivalentTo(
                expectedCarDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarByIdAsync(carId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarAsync(someCar), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarDependencyException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDatetimeOffset();
            Car randomCar = CreateRandomCar(randomDateTime);
            Car someCar = randomCar;
            someCar.CreatedDate = someCar.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageCarException =
                new FailedCarStorageException(databaseUpdateException);

            var expectedCarDependencyException =
                new CarDependencyException(failedStorageCarException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Car> modifyCarTask =
                this.carService.ModifyCarAsync(someCar);

            CarDependencyException actualCarDependencyException =
                await Assert.ThrowsAsync<CarDependencyException>(
                    modifyCarTask.AsTask);

            // then
            actualCarDependencyException.Should().BeEquivalentTo(expectedCarDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarDependencyException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Car randomCar = CreateRandomCar(randomDateTime);
            Car someCar = randomCar;
            someCar.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCarException =
                new LockedCarException(databaseUpdateConcurrencyException);

            var expectedCarDependencyValidationException =
                new CarDependencyValidationException(lockedCarException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Car> modifyCarTask =
                this.carService.ModifyCarAsync(someCar);

            CarDependencyValidationException actualCarDependencyValidationException =
                await Assert.ThrowsAsync<CarDependencyValidationException>(modifyCarTask.AsTask);

            // then
            actualCarDependencyValidationException.Should()
                .BeEquivalentTo(expectedCarDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            var randomDateTime = GetRandomDateTime();
            Car randomCar = CreateRandomCar(randomDateTime);
            Car someCar = randomCar;
            someCar.CreatedDate = someCar.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedCarServiceException =
                new FailedCarServiceException(serviceException);

            var expectedCarServiceException =
                new CarServiceException(failedCarServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Car> modifyCarTask =
                this.carService.ModifyCarAsync(someCar);

            CarServiceException actualCarServiceException =
                await Assert.ThrowsAsync<CarServiceException>(
                    modifyCarTask.AsTask);

            // then
            actualCarServiceException.Should().BeEquivalentTo(expectedCarServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
