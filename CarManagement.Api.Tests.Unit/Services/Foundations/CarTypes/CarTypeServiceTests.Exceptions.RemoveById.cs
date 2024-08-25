// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;
using CarManagement.Api.Models.CarTypes.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarTypes
{
    public partial class CarTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someCarTypeId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCarTypeException =
                new LockedCarTypeException(databaseUpdateConcurrencyException);

            var expectedCarTypeDependencyValidationException =
                new CarTypeDependencyValidationException(lockedCarTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<CarType> removeCarTypeByIdTask =
               this.carTypeService.RemoveCarTypeByIdAsync(someCarTypeId);

            CarTypeDependencyValidationException actualCarTypeDependencyValidationException =
                await Assert.ThrowsAsync<CarTypeDependencyValidationException>(
                    removeCarTypeByIdTask.AsTask);

            // then
            actualCarTypeDependencyValidationException.Should().BeEquivalentTo(
               expectedCarTypeDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCarTypeAsync(It.IsAny<CarType>()), Times.Never);

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

            var failedCarTypeStorageException =
                new FailedCarTypeStorageException(sqlException);

            var expectedCarTypeDependencyException =
                new CarTypeDependencyException(failedCarTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<CarType> removeCarTypeTask =
                this.carTypeService.RemoveCarTypeByIdAsync(someId);

            CarTypeDependencyException actualCarTypeDependencyException =
                await Assert.ThrowsAsync<CarTypeDependencyException>(
                    removeCarTypeTask.AsTask);

            // then
            actualCarTypeDependencyException.Should().BeEquivalentTo(expectedCarTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarTypeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someCarTypeId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCarTypeServiceException =
                new FailedCarTypeServiceException(serviceException);

            var expectedCarTypeServiceException =
                new CarTypeServiceException(failedCarTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(someCarTypeId))
                    .Throws(serviceException);

            // when
            ValueTask<CarType> removeCarTypeByIdTask =
                this.carTypeService.RemoveCarTypeByIdAsync(someCarTypeId);

            CarTypeServiceException actualCarTypeServiceException =
                await Assert.ThrowsAsync<CarTypeServiceException>(
                    removeCarTypeByIdTask.AsTask);

            // then
            actualCarTypeServiceException.Should().BeEquivalentTo(expectedCarTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}