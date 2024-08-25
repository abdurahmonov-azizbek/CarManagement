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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.ServiceTypes
{
    public partial class ServiceTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someServiceTypeId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedServiceTypeException =
                new LockedServiceTypeException(databaseUpdateConcurrencyException);

            var expectedServiceTypeDependencyValidationException =
                new ServiceTypeDependencyValidationException(lockedServiceTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<ServiceType> removeServiceTypeByIdTask =
               this.serviceTypeService.RemoveServiceTypeByIdAsync(someServiceTypeId);

            ServiceTypeDependencyValidationException actualServiceTypeDependencyValidationException =
                await Assert.ThrowsAsync<ServiceTypeDependencyValidationException>(
                    removeServiceTypeByIdTask.AsTask);

            // then
            actualServiceTypeDependencyValidationException.Should().BeEquivalentTo(
               expectedServiceTypeDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteServiceTypeAsync(It.IsAny<ServiceType>()), Times.Never);

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

            var failedServiceTypeStorageException =
                new FailedServiceTypeStorageException(sqlException);

            var expectedServiceTypeDependencyException =
                new ServiceTypeDependencyException(failedServiceTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<ServiceType> removeServiceTypeTask =
                this.serviceTypeService.RemoveServiceTypeByIdAsync(someId);

            ServiceTypeDependencyException actualServiceTypeDependencyException =
                await Assert.ThrowsAsync<ServiceTypeDependencyException>(
                    removeServiceTypeTask.AsTask);

            // then
            actualServiceTypeDependencyException.Should().BeEquivalentTo(expectedServiceTypeDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someServiceTypeId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedServiceTypeServiceException =
                new FailedServiceTypeServiceException(serviceException);

            var expectedServiceTypeServiceException =
                new ServiceTypeServiceException(failedServiceTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(someServiceTypeId))
                    .Throws(serviceException);

            // when
            ValueTask<ServiceType> removeServiceTypeByIdTask =
                this.serviceTypeService.RemoveServiceTypeByIdAsync(someServiceTypeId);

            ServiceTypeServiceException actualServiceTypeServiceException =
                await Assert.ThrowsAsync<ServiceTypeServiceException>(
                    removeServiceTypeByIdTask.AsTask);

            // then
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