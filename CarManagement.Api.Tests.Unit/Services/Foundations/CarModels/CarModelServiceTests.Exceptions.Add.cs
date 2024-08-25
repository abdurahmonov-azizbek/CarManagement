// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;
using CarManagement.Api.Models.CarModels.Exceptions;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarModels
{
    public partial class CarModelServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CarModel someCarModel = CreateRandomCarModel();
            Guid carModelId = someCarModel.Id;
            SqlException sqlException = CreateSqlException();

            FailedCarModelStorageException failedCarModelStorageException =
                new FailedCarModelStorageException(sqlException);

            CarModelDependencyException expectedCarModelDependencyException =
                new CarModelDependencyException(failedCarModelStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<CarModel> addCarModelTask = this.carModelService
                .AddCarModelAsync(someCarModel);

            CarModelDependencyException actualCarModelDependencyException =
                await Assert.ThrowsAsync<CarModelDependencyException>(addCarModelTask.AsTask);

            // then
            actualCarModelDependencyException.Should().BeEquivalentTo(expectedCarModelDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedCarModelDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            CarModel someCarModel = CreateRandomCarModel();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsCarModelException =
                new AlreadyExistsCarModelException(duplicateKeyException);

            var expectedCarModelDependencyValidationException =
                new CarModelDependencyValidationException(alreadyExistsCarModelException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<CarModel> addCarModelTask = this.carModelService.AddCarModelAsync(someCarModel);

            CarModelDependencyValidationException actualCarModelDependencyValidationException =
                await Assert.ThrowsAsync<CarModelDependencyValidationException>(
                    addCarModelTask.AsTask);

            // then
            actualCarModelDependencyValidationException.Should().BeEquivalentTo(
                expectedCarModelDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            CarModel someCarModel = CreateRandomCarModel();
            var dbUpdateException = new DbUpdateException();

            var failedCarModelStorageException = new FailedCarModelStorageException(dbUpdateException);

            var expectedCarModelDependencyException =
                new CarModelDependencyException(failedCarModelStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<CarModel> addCarModelTask = this.carModelService.AddCarModelAsync(someCarModel);

            CarModelDependencyException actualCarModelDependencyException =
                 await Assert.ThrowsAsync<CarModelDependencyException>(addCarModelTask.AsTask);

            // then
            actualCarModelDependencyException.Should()
                .BeEquivalentTo(expectedCarModelDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedCarModelDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarModelAsync(It.IsAny<CarModel>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            CarModel someCarModel = CreateRandomCarModel();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedCarModelStorageException =
                new FailedCarModelStorageException(dbUpdateException);

            var expectedCarModelDependencyException =
                new CarModelDependencyException(failedCarModelStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<CarModel> addCarModelTask =
                this.carModelService.AddCarModelAsync(someCarModel);

            CarModelDependencyException actualCarModelDependencyException =
                await Assert.ThrowsAsync<CarModelDependencyException>(addCarModelTask.AsTask);

            // then
            actualCarModelDependencyException.Should().BeEquivalentTo(expectedCarModelDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            CarModel someCarModel = CreateRandomCarModel();
            var serviceException = new Exception();
            var failedCarModelException = new FailedCarModelServiceException(serviceException);

            var expectedCarModelServiceExceptions =
                new CarModelServiceException(failedCarModelException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<CarModel> addCarModelTask = this.carModelService.AddCarModelAsync(someCarModel);

            CarModelServiceException actualCarModelServiceException =
                await Assert.ThrowsAsync<CarModelServiceException>(addCarModelTask.AsTask);

            //then
            actualCarModelServiceException.Should().BeEquivalentTo(
                expectedCarModelServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}