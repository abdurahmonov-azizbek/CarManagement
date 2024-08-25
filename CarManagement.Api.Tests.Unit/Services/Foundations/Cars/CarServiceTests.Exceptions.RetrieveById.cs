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
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Cars
{
    public partial class CarServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedCarStorageException =
                new FailedCarStorageException(sqlException);

            var expectedCarDependencyException =
                new CarDependencyException(failedCarStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<Car> retrieveCarByIdTask =
                this.carService.RetrieveCarByIdAsync(someId);

            CarDependencyException actualCarDependencyexception =
                await Assert.ThrowsAsync<CarDependencyException>(
                    retrieveCarByIdTask.AsTask);

            //then
            actualCarDependencyexception.Should().BeEquivalentTo(
                expectedCarDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCarServiceException =
                new FailedCarServiceException(serviceException);

            var expectedCarServiceException =
                new CarServiceException(failedCarServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<Car> retrieveCarByIdTask =
                this.carService.RetrieveCarByIdAsync(someId);

            CarServiceException actualCarServiceException =
                await Assert.ThrowsAsync<CarServiceException>(retrieveCarByIdTask.AsTask);

            //then
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