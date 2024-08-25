// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedServiceTypeStorageException =
                new FailedServiceTypeStorageException(sqlException);

            var expectedServiceTypeDependencyException =
                new ServiceTypeDependencyException(failedServiceTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<ServiceType> retrieveServiceTypeByIdTask =
                this.serviceTypeService.RetrieveServiceTypeByIdAsync(someId);

            ServiceTypeDependencyException actualServiceTypeDependencyexception =
                await Assert.ThrowsAsync<ServiceTypeDependencyException>(
                    retrieveServiceTypeByIdTask.AsTask);

            //then
            actualServiceTypeDependencyexception.Should().BeEquivalentTo(
                expectedServiceTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedServiceTypeDependencyException))), Times.Once);

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

            var failedServiceTypeServiceException =
                new FailedServiceTypeServiceException(serviceException);

            var expectedServiceTypeServiceException =
                new ServiceTypeServiceException(failedServiceTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<ServiceType> retrieveServiceTypeByIdTask =
                this.serviceTypeService.RetrieveServiceTypeByIdAsync(someId);

            ServiceTypeServiceException actualServiceTypeServiceException =
                await Assert.ThrowsAsync<ServiceTypeServiceException>(retrieveServiceTypeByIdTask.AsTask);

            //then
            actualServiceTypeServiceException.Should().BeEquivalentTo(expectedServiceTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}