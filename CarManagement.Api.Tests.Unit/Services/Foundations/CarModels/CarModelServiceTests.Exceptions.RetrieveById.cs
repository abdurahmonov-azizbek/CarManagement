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
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarModels
{
    public partial class CarModelServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedCarModelStorageException =
                new FailedCarModelStorageException(sqlException);

            var expectedCarModelDependencyException =
                new CarModelDependencyException(failedCarModelStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<CarModel> retrieveCarModelByIdTask =
                this.carModelService.RetrieveCarModelByIdAsync(someId);

            CarModelDependencyException actualCarModelDependencyexception =
                await Assert.ThrowsAsync<CarModelDependencyException>(
                    retrieveCarModelByIdTask.AsTask);

            //then
            actualCarModelDependencyexception.Should().BeEquivalentTo(
                expectedCarModelDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCarModelServiceException =
                new FailedCarModelServiceException(serviceException);

            var expectedCarModelServiceException =
                new CarModelServiceException(failedCarModelServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<CarModel> retrieveCarModelByIdTask =
                this.carModelService.RetrieveCarModelByIdAsync(someId);

            CarModelServiceException actualCarModelServiceException =
                await Assert.ThrowsAsync<CarModelServiceException>(retrieveCarModelByIdTask.AsTask);

            //then
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