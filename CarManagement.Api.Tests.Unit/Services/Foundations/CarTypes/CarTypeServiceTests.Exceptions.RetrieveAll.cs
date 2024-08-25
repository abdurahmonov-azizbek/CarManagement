// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedCarTypeStorageException(sqlException);

            var expectedCarTypeDependencyException =
                new CarTypeDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCarTypes()).Throws(sqlException);

            //when
            Action retrieveAllCarTypesAction = () =>
                this.carTypeService.RetrieveAllCarTypes();

            CarTypeDependencyException actualCarTypeDependencyException =
                Assert.Throws<CarTypeDependencyException>(retrieveAllCarTypesAction);

            //then
            actualCarTypeDependencyException.Should().BeEquivalentTo(
                expectedCarTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCarTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCarTypeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedCarTypeServiceException =
                new FailedCarTypeServiceException(serviceException);

            var expectedCarTypeServiceException =
                new CarTypeServiceException(failedCarTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCarTypes()).Throws(serviceException);

            // when
            Action retrieveAllCarTypesAction = () =>
                this.carTypeService.RetrieveAllCarTypes();

            CarTypeServiceException actualCarTypeServiceException =
                Assert.Throws<CarTypeServiceException>(retrieveAllCarTypesAction);

            // then
            actualCarTypeServiceException.Should().BeEquivalentTo(expectedCarTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCarTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}