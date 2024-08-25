// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedCarStorageException(sqlException);

            var expectedCarDependencyException =
                new CarDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCars()).Throws(sqlException);

            //when
            Action retrieveAllCarsAction = () =>
                this.carService.RetrieveAllCars();

            CarDependencyException actualCarDependencyException =
                Assert.Throws<CarDependencyException>(retrieveAllCarsAction);

            //then
            actualCarDependencyException.Should().BeEquivalentTo(
                expectedCarDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCars(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedCarServiceException =
                new FailedCarServiceException(serviceException);

            var expectedCarServiceException =
                new CarServiceException(failedCarServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCars()).Throws(serviceException);

            // when
            Action retrieveAllCarsAction = () =>
                this.carService.RetrieveAllCars();

            CarServiceException actualCarServiceException =
                Assert.Throws<CarServiceException>(retrieveAllCarsAction);

            // then
            actualCarServiceException.Should().BeEquivalentTo(expectedCarServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCars(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}