// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedCarModelStorageException(sqlException);

            var expectedCarModelDependencyException =
                new CarModelDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCarModels()).Throws(sqlException);

            //when
            Action retrieveAllCarModelsAction = () =>
                this.carModelService.RetrieveAllCarModels();

            CarModelDependencyException actualCarModelDependencyException =
                Assert.Throws<CarModelDependencyException>(retrieveAllCarModelsAction);

            //then
            actualCarModelDependencyException.Should().BeEquivalentTo(
                expectedCarModelDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCarModels(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarModelDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedCarModelServiceException =
                new FailedCarModelServiceException(serviceException);

            var expectedCarModelServiceException =
                new CarModelServiceException(failedCarModelServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCarModels()).Throws(serviceException);

            // when
            Action retrieveAllCarModelsAction = () =>
                this.carModelService.RetrieveAllCarModels();

            CarModelServiceException actualCarModelServiceException =
                Assert.Throws<CarModelServiceException>(retrieveAllCarModelsAction);

            // then
            actualCarModelServiceException.Should().BeEquivalentTo(expectedCarModelServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCarModels(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarModelServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}