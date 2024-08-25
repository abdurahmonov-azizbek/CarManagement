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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Services
{
    public partial class ServiceServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someServiceId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedServiceException =
                new LockedServiceException(databaseUpdateConcurrencyException);

            var expectedServiceDependencyValidationException =
                new ServiceDependencyValidationException(lockedServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Service> removeServiceByIdTask =
               this.serviceService.RemoveServiceByIdAsync(someServiceId);

            ServiceDependencyValidationException actualServiceDependencyValidationException =
                await Assert.ThrowsAsync<ServiceDependencyValidationException>(
                    removeServiceByIdTask.AsTask);

            // then
            actualServiceDependencyValidationException.Should().BeEquivalentTo(
               expectedServiceDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteServiceAsync(It.IsAny<Service>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedServiceStorageException =
                new FailedServiceStorageException(sqlException);

            var expectedServiceDependencyException =
                new ServiceDependencyException(failedServiceStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<Service> removeServiceTask =
                this.serviceService.RemoveServiceByIdAsync(someId);

            ServiceDependencyException actualServiceDependencyException =
                await Assert.ThrowsAsync<ServiceDependencyException>(
                    removeServiceTask.AsTask);

            // then
            actualServiceDependencyException.Should().BeEquivalentTo(expectedServiceDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someServiceId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedServiceServiceException =
                new FailedServiceServiceException(serviceException);

            var expectedServiceServiceException =
                new ServiceServiceException(failedServiceServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(someServiceId))
                    .Throws(serviceException);

            // when
            ValueTask<Service> removeServiceByIdTask =
                this.serviceService.RemoveServiceByIdAsync(someServiceId);

            ServiceServiceException actualServiceServiceException =
                await Assert.ThrowsAsync<ServiceServiceException>(
                    removeServiceByIdTask.AsTask);

            // then
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