// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Services.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Services
{
    public partial class ServiceServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedServiceStorageException(sqlException);

            var expectedServiceDependencyException =
                new ServiceDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllServices()).Throws(sqlException);

            //when
            Action retrieveAllServicesAction = () =>
                this.serviceService.RetrieveAllServices();

            ServiceDependencyException actualServiceDependencyException =
                Assert.Throws<ServiceDependencyException>(retrieveAllServicesAction);

            //then
            actualServiceDependencyException.Should().BeEquivalentTo(
                expectedServiceDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllServices(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedServiceDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedServiceServiceException =
                new FailedServiceServiceException(serviceException);

            var expectedServiceServiceException =
                new ServiceServiceException(failedServiceServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllServices()).Throws(serviceException);

            // when
            Action retrieveAllServicesAction = () =>
                this.serviceService.RetrieveAllServices();

            ServiceServiceException actualServiceServiceException =
                Assert.Throws<ServiceServiceException>(retrieveAllServicesAction);

            // then
            actualServiceServiceException.Should().BeEquivalentTo(expectedServiceServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllServices(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}