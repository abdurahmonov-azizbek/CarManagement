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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            CarType randomCarType = CreateRandomCarType();
            CarType someCarType = randomCarType;
            Guid carTypeId = someCarType.Id;
            SqlException sqlException = CreateSqlException();

            var failedCarTypeStorageException =
                new FailedCarTypeStorageException(sqlException);

            var expectedCarTypeDependencyException =
                new CarTypeDependencyException(failedCarTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(someCarType);

            CarTypeDependencyException actualCarTypeDependencyException =
                await Assert.ThrowsAsync<CarTypeDependencyException>(
                    modifyCarTypeTask.AsTask);

            // then
            actualCarTypeDependencyException.Should().BeEquivalentTo(
                expectedCarTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(carTypeId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarTypeAsync(someCarType), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarTypeDependencyException))), Times.Once);

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
            CarType randomCarType = CreateRandomCarType(randomDateTime);
            CarType someCarType = randomCarType;
            someCarType.CreatedDate = someCarType.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageCarTypeException =
                new FailedCarTypeStorageException(databaseUpdateException);

            var expectedCarTypeDependencyException =
                new CarTypeDependencyException(failedStorageCarTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(someCarType);

            CarTypeDependencyException actualCarTypeDependencyException =
                await Assert.ThrowsAsync<CarTypeDependencyException>(
                    modifyCarTypeTask.AsTask);

            // then
            actualCarTypeDependencyException.Should().BeEquivalentTo(expectedCarTypeDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeDependencyException))), Times.Once);

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
            CarType randomCarType = CreateRandomCarType(randomDateTime);
            CarType someCarType = randomCarType;
            someCarType.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCarTypeException =
                new LockedCarTypeException(databaseUpdateConcurrencyException);

            var expectedCarTypeDependencyValidationException =
                new CarTypeDependencyValidationException(lockedCarTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(someCarType);

            CarTypeDependencyValidationException actualCarTypeDependencyValidationException =
                await Assert.ThrowsAsync<CarTypeDependencyValidationException>(modifyCarTypeTask.AsTask);

            // then
            actualCarTypeDependencyValidationException.Should()
                .BeEquivalentTo(expectedCarTypeDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeDependencyValidationException))), Times.Once);

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
            CarType randomCarType = CreateRandomCarType(randomDateTime);
            CarType someCarType = randomCarType;
            someCarType.CreatedDate = someCarType.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedCarTypeServiceException =
                new FailedCarTypeServiceException(serviceException);

            var expectedCarTypeServiceException =
                new CarTypeServiceException(failedCarTypeServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<CarType> modifyCarTypeTask =
                this.carTypeService.ModifyCarTypeAsync(someCarType);

            CarTypeServiceException actualCarTypeServiceException =
                await Assert.ThrowsAsync<CarTypeServiceException>(
                    modifyCarTypeTask.AsTask);

            // then
            actualCarTypeServiceException.Should().BeEquivalentTo(expectedCarTypeServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
