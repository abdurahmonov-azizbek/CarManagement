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
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarTypes
{
    public partial class CarTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedCarTypeStorageException =
                new FailedCarTypeStorageException(sqlException);

            var expectedCarTypeDependencyException =
                new CarTypeDependencyException(failedCarTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<CarType> retrieveCarTypeByIdTask =
                this.carTypeService.RetrieveCarTypeByIdAsync(someId);

            CarTypeDependencyException actualCarTypeDependencyexception =
                await Assert.ThrowsAsync<CarTypeDependencyException>(
                    retrieveCarTypeByIdTask.AsTask);

            //then
            actualCarTypeDependencyexception.Should().BeEquivalentTo(
                expectedCarTypeDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCarTypeServiceException =
                new FailedCarTypeServiceException(serviceException);

            var expectedCarTypeServiceException =
                new CarTypeServiceException(failedCarTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<CarType> retrieveCarTypeByIdTask =
                this.carTypeService.RetrieveCarTypeByIdAsync(someId);

            CarTypeServiceException actualCarTypeServiceException =
                await Assert.ThrowsAsync<CarTypeServiceException>(retrieveCarTypeByIdTask.AsTask);

            //then
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