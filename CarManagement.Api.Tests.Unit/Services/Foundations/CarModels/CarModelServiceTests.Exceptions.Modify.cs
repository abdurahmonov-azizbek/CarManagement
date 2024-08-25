// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;
using CarManagement.Api.Models.CarModels.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            CarModel randomCarModel = CreateRandomCarModel();
            CarModel someCarModel = randomCarModel;
            Guid carModelId = someCarModel.Id;
            SqlException sqlException = CreateSqlException();

            var failedCarModelStorageException =
                new FailedCarModelStorageException(sqlException);

            var expectedCarModelDependencyException =
                new CarModelDependencyException(failedCarModelStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(someCarModel);

            CarModelDependencyException actualCarModelDependencyException =
                await Assert.ThrowsAsync<CarModelDependencyException>(
                    modifyCarModelTask.AsTask);

            // then
            actualCarModelDependencyException.Should().BeEquivalentTo(
                expectedCarModelDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(carModelId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarModelAsync(someCarModel), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarModelDependencyException))), Times.Once);

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
            CarModel randomCarModel = CreateRandomCarModel(randomDateTime);
            CarModel someCarModel = randomCarModel;
            someCarModel.CreatedDate = someCarModel.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageCarModelException =
                new FailedCarModelStorageException(databaseUpdateException);

            var expectedCarModelDependencyException =
                new CarModelDependencyException(failedStorageCarModelException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(someCarModel);

            CarModelDependencyException actualCarModelDependencyException =
                await Assert.ThrowsAsync<CarModelDependencyException>(
                    modifyCarModelTask.AsTask);

            // then
            actualCarModelDependencyException.Should().BeEquivalentTo(expectedCarModelDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelDependencyException))), Times.Once);

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
            CarModel randomCarModel = CreateRandomCarModel(randomDateTime);
            CarModel someCarModel = randomCarModel;
            someCarModel.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCarModelException =
                new LockedCarModelException(databaseUpdateConcurrencyException);

            var expectedCarModelDependencyValidationException =
                new CarModelDependencyValidationException(lockedCarModelException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(someCarModel);

            CarModelDependencyValidationException actualCarModelDependencyValidationException =
                await Assert.ThrowsAsync<CarModelDependencyValidationException>(modifyCarModelTask.AsTask);

            // then
            actualCarModelDependencyValidationException.Should()
                .BeEquivalentTo(expectedCarModelDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelDependencyValidationException))), Times.Once);

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
            CarModel randomCarModel = CreateRandomCarModel(randomDateTime);
            CarModel someCarModel = randomCarModel;
            someCarModel.CreatedDate = someCarModel.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedCarModelServiceException =
                new FailedCarModelServiceException(serviceException);

            var expectedCarModelServiceException =
                new CarModelServiceException(failedCarModelServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<CarModel> modifyCarModelTask =
                this.carModelService.ModifyCarModelAsync(someCarModel);

            CarModelServiceException actualCarModelServiceException =
                await Assert.ThrowsAsync<CarModelServiceException>(
                    modifyCarModelTask.AsTask);

            // then
            actualCarModelServiceException.Should().BeEquivalentTo(expectedCarModelServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
