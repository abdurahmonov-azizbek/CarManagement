// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedServiceStorageException =
                new FailedServiceStorageException(sqlException);

            var expectedServiceDependencyException =
                new ServiceDependencyException(failedServiceStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<Service> retrieveServiceByIdTask =
                this.serviceService.RetrieveServiceByIdAsync(someId);

            ServiceDependencyException actualServiceDependencyexception =
                await Assert.ThrowsAsync<ServiceDependencyException>(
                    retrieveServiceByIdTask.AsTask);

            //then
            actualServiceDependencyexception.Should().BeEquivalentTo(
                expectedServiceDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedServiceDependencyException))), Times.Once);

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

            var failedServiceServiceException =
                new FailedServiceServiceException(serviceException);

            var expectedServiceServiceException =
                new ServiceServiceException(failedServiceServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<Service> retrieveServiceByIdTask =
                this.serviceService.RetrieveServiceByIdAsync(someId);

            ServiceServiceException actualServiceServiceException =
                await Assert.ThrowsAsync<ServiceServiceException>(retrieveServiceByIdTask.AsTask);

            //then
            actualServiceServiceException.Should().BeEquivalentTo(expectedServiceServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}