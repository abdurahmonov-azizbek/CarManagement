// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Cars;
using CarManagement.Api.Models.Cars.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            Car someCar = CreateRandomCar();
            Guid carId = someCar.Id;
            SqlException sqlException = CreateSqlException();

            FailedCarStorageException failedCarStorageException =
                new FailedCarStorageException(sqlException);

            CarDependencyException expectedCarDependencyException =
                new CarDependencyException(failedCarStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Car> addCarTask = this.carService
                .AddCarAsync(someCar);

            CarDependencyException actualCarDependencyException =
                await Assert.ThrowsAsync<CarDependencyException>(addCarTask.AsTask);

            // then
            actualCarDependencyException.Should().BeEquivalentTo(expectedCarDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedCarDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            Car someCar = CreateRandomCar();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsCarException =
                new AlreadyExistsCarException(duplicateKeyException);

            var expectedCarDependencyValidationException =
                new CarDependencyValidationException(alreadyExistsCarException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<Car> addCarTask = this.carService.AddCarAsync(someCar);

            CarDependencyValidationException actualCarDependencyValidationException =
                await Assert.ThrowsAsync<CarDependencyValidationException>(
                    addCarTask.AsTask);

            // then
            actualCarDependencyValidationException.Should().BeEquivalentTo(
                expectedCarDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            Car someCar = CreateRandomCar();
            var dbUpdateException = new DbUpdateException();

            var failedCarStorageException = new FailedCarStorageException(dbUpdateException);

            var expectedCarDependencyException =
                new CarDependencyException(failedCarStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Car> addCarTask = this.carService.AddCarAsync(someCar);

            CarDependencyException actualCarDependencyException =
                 await Assert.ThrowsAsync<CarDependencyException>(addCarTask.AsTask);

            // then
            actualCarDependencyException.Should()
                .BeEquivalentTo(expectedCarDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedCarDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarAsync(It.IsAny<Car>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            Car someCar = CreateRandomCar();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedCarStorageException =
                new FailedCarStorageException(dbUpdateException);

            var expectedCarDependencyException =
                new CarDependencyException(failedCarStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<Car> addCarTask =
                this.carService.AddCarAsync(someCar);

            CarDependencyException actualCarDependencyException =
                await Assert.ThrowsAsync<CarDependencyException>(addCarTask.AsTask);

            // then
            actualCarDependencyException.Should().BeEquivalentTo(expectedCarDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            Car someCar = CreateRandomCar();
            var serviceException = new Exception();
            var failedCarException = new FailedCarServiceException(serviceException);

            var expectedCarServiceExceptions =
                new CarServiceException(failedCarException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<Car> addCarTask = this.carService.AddCarAsync(someCar);

            CarServiceException actualCarServiceException =
                await Assert.ThrowsAsync<CarServiceException>(addCarTask.AsTask);

            //then
            actualCarServiceException.Should().BeEquivalentTo(
                expectedCarServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}