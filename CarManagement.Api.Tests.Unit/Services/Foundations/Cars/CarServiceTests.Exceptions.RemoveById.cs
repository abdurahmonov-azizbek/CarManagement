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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someCarId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCarException =
                new LockedCarException(databaseUpdateConcurrencyException);

            var expectedCarDependencyValidationException =
                new CarDependencyValidationException(lockedCarException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Car> removeCarByIdTask =
               this.carService.RemoveCarByIdAsync(someCarId);

            CarDependencyValidationException actualCarDependencyValidationException =
                await Assert.ThrowsAsync<CarDependencyValidationException>(
                    removeCarByIdTask.AsTask);

            // then
            actualCarDependencyValidationException.Should().BeEquivalentTo(
               expectedCarDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCarAsync(It.IsAny<Car>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedCarStorageException =
                new FailedCarStorageException(sqlException);

            var expectedCarDependencyException =
                new CarDependencyException(failedCarStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<Car> removeCarTask =
                this.carService.RemoveCarByIdAsync(someId);

            CarDependencyException actualCarDependencyException =
                await Assert.ThrowsAsync<CarDependencyException>(
                    removeCarTask.AsTask);

            // then
            actualCarDependencyException.Should().BeEquivalentTo(expectedCarDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someCarId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCarServiceException =
                new FailedCarServiceException(serviceException);

            var expectedCarServiceException =
                new CarServiceException(failedCarServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarByIdAsync(someCarId))
                    .Throws(serviceException);

            // when
            ValueTask<Car> removeCarByIdTask =
                this.carService.RemoveCarByIdAsync(someCarId);

            CarServiceException actualCarServiceException =
                await Assert.ThrowsAsync<CarServiceException>(
                    removeCarByIdTask.AsTask);

            // then
            actualCarServiceException.Should().BeEquivalentTo(expectedCarServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}