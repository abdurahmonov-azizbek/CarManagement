// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;
using CarManagement.Api.Models.Services.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            Service someService = CreateRandomService();
            Guid serviceId = someService.Id;
            SqlException sqlException = CreateSqlException();

            FailedServiceStorageException failedServiceStorageException =
                new FailedServiceStorageException(sqlException);

            ServiceDependencyException expectedServiceDependencyException =
                new ServiceDependencyException(failedServiceStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Service> addServiceTask = this.serviceService
                .AddServiceAsync(someService);

            ServiceDependencyException actualServiceDependencyException =
                await Assert.ThrowsAsync<ServiceDependencyException>(addServiceTask.AsTask);

            // then
            actualServiceDependencyException.Should().BeEquivalentTo(expectedServiceDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedServiceDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            Service someService = CreateRandomService();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsServiceException =
                new AlreadyExistsServiceException(duplicateKeyException);

            var expectedServiceDependencyValidationException =
                new ServiceDependencyValidationException(alreadyExistsServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<Service> addServiceTask = this.serviceService.AddServiceAsync(someService);

            ServiceDependencyValidationException actualServiceDependencyValidationException =
                await Assert.ThrowsAsync<ServiceDependencyValidationException>(
                    addServiceTask.AsTask);

            // then
            actualServiceDependencyValidationException.Should().BeEquivalentTo(
                expectedServiceDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            Service someService = CreateRandomService();
            var dbUpdateException = new DbUpdateException();

            var failedServiceStorageException = new FailedServiceStorageException(dbUpdateException);

            var expectedServiceDependencyException =
                new ServiceDependencyException(failedServiceStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Service> addServiceTask = this.serviceService.AddServiceAsync(someService);

            ServiceDependencyException actualServiceDependencyException =
                 await Assert.ThrowsAsync<ServiceDependencyException>(addServiceTask.AsTask);

            // then
            actualServiceDependencyException.Should()
                .BeEquivalentTo(expectedServiceDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedServiceDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateServiceAsync(It.IsAny<Service>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            Service someService = CreateRandomService();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedServiceStorageException =
                new FailedServiceStorageException(dbUpdateException);

            var expectedServiceDependencyException =
                new ServiceDependencyException(failedServiceStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<Service> addServiceTask =
                this.serviceService.AddServiceAsync(someService);

            ServiceDependencyException actualServiceDependencyException =
                await Assert.ThrowsAsync<ServiceDependencyException>(addServiceTask.AsTask);

            // then
            actualServiceDependencyException.Should().BeEquivalentTo(expectedServiceDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            Service someService = CreateRandomService();
            var serviceException = new Exception();
            var failedServiceException = new FailedServiceServiceException(serviceException);

            var expectedServiceServiceExceptions =
                new ServiceServiceException(failedServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<Service> addServiceTask = this.serviceService.AddServiceAsync(someService);

            ServiceServiceException actualServiceServiceException =
                await Assert.ThrowsAsync<ServiceServiceException>(addServiceTask.AsTask);

            //then
            actualServiceServiceException.Should().BeEquivalentTo(
                expectedServiceServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}