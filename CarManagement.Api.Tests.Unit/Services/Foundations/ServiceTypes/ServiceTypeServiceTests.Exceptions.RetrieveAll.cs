// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.ServiceTypes.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.ServiceTypes
{
    public partial class ServiceTypeServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedServiceTypeStorageException(sqlException);

            var expectedServiceTypeDependencyException =
                new ServiceTypeDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllServiceTypes()).Throws(sqlException);

            //when
            Action retrieveAllServiceTypesAction = () =>
                this.serviceTypeService.RetrieveAllServiceTypes();

            ServiceTypeDependencyException actualServiceTypeDependencyException =
                Assert.Throws<ServiceTypeDependencyException>(retrieveAllServiceTypesAction);

            //then
            actualServiceTypeDependencyException.Should().BeEquivalentTo(
                expectedServiceTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllServiceTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedServiceTypeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedServiceTypeServiceException =
                new FailedServiceTypeServiceException(serviceException);

            var expectedServiceTypeServiceException =
                new ServiceTypeServiceException(failedServiceTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllServiceTypes()).Throws(serviceException);

            // when
            Action retrieveAllServiceTypesAction = () =>
                this.serviceTypeService.RetrieveAllServiceTypes();

            ServiceTypeServiceException actualServiceTypeServiceException =
                Assert.Throws<ServiceTypeServiceException>(retrieveAllServiceTypesAction);

            // then
            actualServiceTypeServiceException.Should().BeEquivalentTo(expectedServiceTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllServiceTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}