// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;
using CarManagement.Api.Models.CarTypes.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CarType someCarType = CreateRandomCarType();
            Guid carTypeId = someCarType.Id;
            SqlException sqlException = CreateSqlException();

            FailedCarTypeStorageException failedCarTypeStorageException =
                new FailedCarTypeStorageException(sqlException);

            CarTypeDependencyException expectedCarTypeDependencyException =
                new CarTypeDependencyException(failedCarTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<CarType> addCarTypeTask = this.carTypeService
                .AddCarTypeAsync(someCarType);

            CarTypeDependencyException actualCarTypeDependencyException =
                await Assert.ThrowsAsync<CarTypeDependencyException>(addCarTypeTask.AsTask);

            // then
            actualCarTypeDependencyException.Should().BeEquivalentTo(expectedCarTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedCarTypeDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            CarType someCarType = CreateRandomCarType();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsCarTypeException =
                new AlreadyExistsCarTypeException(duplicateKeyException);

            var expectedCarTypeDependencyValidationException =
                new CarTypeDependencyValidationException(alreadyExistsCarTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<CarType> addCarTypeTask = this.carTypeService.AddCarTypeAsync(someCarType);

            CarTypeDependencyValidationException actualCarTypeDependencyValidationException =
                await Assert.ThrowsAsync<CarTypeDependencyValidationException>(
                    addCarTypeTask.AsTask);

            // then
            actualCarTypeDependencyValidationException.Should().BeEquivalentTo(
                expectedCarTypeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            CarType someCarType = CreateRandomCarType();
            var dbUpdateException = new DbUpdateException();

            var failedCarTypeStorageException = new FailedCarTypeStorageException(dbUpdateException);

            var expectedCarTypeDependencyException =
                new CarTypeDependencyException(failedCarTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<CarType> addCarTypeTask = this.carTypeService.AddCarTypeAsync(someCarType);

            CarTypeDependencyException actualCarTypeDependencyException =
                 await Assert.ThrowsAsync<CarTypeDependencyException>(addCarTypeTask.AsTask);

            // then
            actualCarTypeDependencyException.Should()
                .BeEquivalentTo(expectedCarTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedCarTypeDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarTypeAsync(It.IsAny<CarType>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            CarType someCarType = CreateRandomCarType();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedCarTypeStorageException =
                new FailedCarTypeStorageException(dbUpdateException);

            var expectedCarTypeDependencyException =
                new CarTypeDependencyException(failedCarTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<CarType> addCarTypeTask =
                this.carTypeService.AddCarTypeAsync(someCarType);

            CarTypeDependencyException actualCarTypeDependencyException =
                await Assert.ThrowsAsync<CarTypeDependencyException>(addCarTypeTask.AsTask);

            // then
            actualCarTypeDependencyException.Should().BeEquivalentTo(expectedCarTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            CarType someCarType = CreateRandomCarType();
            var serviceException = new Exception();
            var failedCarTypeException = new FailedCarTypeServiceException(serviceException);

            var expectedCarTypeServiceExceptions =
                new CarTypeServiceException(failedCarTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<CarType> addCarTypeTask = this.carTypeService.AddCarTypeAsync(someCarType);

            CarTypeServiceException actualCarTypeServiceException =
                await Assert.ThrowsAsync<CarTypeServiceException>(addCarTypeTask.AsTask);

            //then
            actualCarTypeServiceException.Should().BeEquivalentTo(
                expectedCarTypeServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}