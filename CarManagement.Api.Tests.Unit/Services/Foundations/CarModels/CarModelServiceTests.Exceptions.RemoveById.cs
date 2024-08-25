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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someCarModelId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCarModelException =
                new LockedCarModelException(databaseUpdateConcurrencyException);

            var expectedCarModelDependencyValidationException =
                new CarModelDependencyValidationException(lockedCarModelException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<CarModel> removeCarModelByIdTask =
               this.carModelService.RemoveCarModelByIdAsync(someCarModelId);

            CarModelDependencyValidationException actualCarModelDependencyValidationException =
                await Assert.ThrowsAsync<CarModelDependencyValidationException>(
                    removeCarModelByIdTask.AsTask);

            // then
            actualCarModelDependencyValidationException.Should().BeEquivalentTo(
               expectedCarModelDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCarModelAsync(It.IsAny<CarModel>()), Times.Never);

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

            var failedCarModelStorageException =
                new FailedCarModelStorageException(sqlException);

            var expectedCarModelDependencyException =
                new CarModelDependencyException(failedCarModelStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<CarModel> removeCarModelTask =
                this.carModelService.RemoveCarModelByIdAsync(someId);

            CarModelDependencyException actualCarModelDependencyException =
                await Assert.ThrowsAsync<CarModelDependencyException>(
                    removeCarModelTask.AsTask);

            // then
            actualCarModelDependencyException.Should().BeEquivalentTo(expectedCarModelDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarModelDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someCarModelId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCarModelServiceException =
                new FailedCarModelServiceException(serviceException);

            var expectedCarModelServiceException =
                new CarModelServiceException(failedCarModelServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(someCarModelId))
                    .Throws(serviceException);

            // when
            ValueTask<CarModel> removeCarModelByIdTask =
                this.carModelService.RemoveCarModelByIdAsync(someCarModelId);

            CarModelServiceException actualCarModelServiceException =
                await Assert.ThrowsAsync<CarModelServiceException>(
                    removeCarModelByIdTask.AsTask);

            // then
            actualCarModelServiceException.Should().BeEquivalentTo(expectedCarModelServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}