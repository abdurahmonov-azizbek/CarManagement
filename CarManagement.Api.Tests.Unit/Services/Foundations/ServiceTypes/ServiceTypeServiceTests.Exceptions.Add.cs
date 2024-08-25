// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;
using CarManagement.Api.Models.ServiceTypes.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            ServiceType someServiceType = CreateRandomServiceType();
            Guid serviceTypeId = someServiceType.Id;
            SqlException sqlException = CreateSqlException();

            FailedServiceTypeStorageException failedServiceTypeStorageException =
                new FailedServiceTypeStorageException(sqlException);

            ServiceTypeDependencyException expectedServiceTypeDependencyException =
                new ServiceTypeDependencyException(failedServiceTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<ServiceType> addServiceTypeTask = this.serviceTypeService
                .AddServiceTypeAsync(someServiceType);

            ServiceTypeDependencyException actualServiceTypeDependencyException =
                await Assert.ThrowsAsync<ServiceTypeDependencyException>(addServiceTypeTask.AsTask);

            // then
            actualServiceTypeDependencyException.Should().BeEquivalentTo(expectedServiceTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedServiceTypeDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            ServiceType someServiceType = CreateRandomServiceType();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsServiceTypeException =
                new AlreadyExistsServiceTypeException(duplicateKeyException);

            var expectedServiceTypeDependencyValidationException =
                new ServiceTypeDependencyValidationException(alreadyExistsServiceTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<ServiceType> addServiceTypeTask = this.serviceTypeService.AddServiceTypeAsync(someServiceType);

            ServiceTypeDependencyValidationException actualServiceTypeDependencyValidationException =
                await Assert.ThrowsAsync<ServiceTypeDependencyValidationException>(
                    addServiceTypeTask.AsTask);

            // then
            actualServiceTypeDependencyValidationException.Should().BeEquivalentTo(
                expectedServiceTypeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            ServiceType someServiceType = CreateRandomServiceType();
            var dbUpdateException = new DbUpdateException();

            var failedServiceTypeStorageException = new FailedServiceTypeStorageException(dbUpdateException);

            var expectedServiceTypeDependencyException =
                new ServiceTypeDependencyException(failedServiceTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<ServiceType> addServiceTypeTask = this.serviceTypeService.AddServiceTypeAsync(someServiceType);

            ServiceTypeDependencyException actualServiceTypeDependencyException =
                 await Assert.ThrowsAsync<ServiceTypeDependencyException>(addServiceTypeTask.AsTask);

            // then
            actualServiceTypeDependencyException.Should()
                .BeEquivalentTo(expectedServiceTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedServiceTypeDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateServiceTypeAsync(It.IsAny<ServiceType>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            ServiceType someServiceType = CreateRandomServiceType();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedServiceTypeStorageException =
                new FailedServiceTypeStorageException(dbUpdateException);

            var expectedServiceTypeDependencyException =
                new ServiceTypeDependencyException(failedServiceTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<ServiceType> addServiceTypeTask =
                this.serviceTypeService.AddServiceTypeAsync(someServiceType);

            ServiceTypeDependencyException actualServiceTypeDependencyException =
                await Assert.ThrowsAsync<ServiceTypeDependencyException>(addServiceTypeTask.AsTask);

            // then
            actualServiceTypeDependencyException.Should().BeEquivalentTo(expectedServiceTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            ServiceType someServiceType = CreateRandomServiceType();
            var serviceException = new Exception();
            var failedServiceTypeException = new FailedServiceTypeServiceException(serviceException);

            var expectedServiceTypeServiceExceptions =
                new ServiceTypeServiceException(failedServiceTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<ServiceType> addServiceTypeTask = this.serviceTypeService.AddServiceTypeAsync(someServiceType);

            ServiceTypeServiceException actualServiceTypeServiceException =
                await Assert.ThrowsAsync<ServiceTypeServiceException>(addServiceTypeTask.AsTask);

            //then
            actualServiceTypeServiceException.Should().BeEquivalentTo(
                expectedServiceTypeServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}