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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            Service randomService = CreateRandomService();
            Service someService = randomService;
            Guid serviceId = someService.Id;
            SqlException sqlException = CreateSqlException();

            var failedServiceStorageException =
                new FailedServiceStorageException(sqlException);

            var expectedServiceDependencyException =
                new ServiceDependencyException(failedServiceStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(someService);

            ServiceDependencyException actualServiceDependencyException =
                await Assert.ThrowsAsync<ServiceDependencyException>(
                    modifyServiceTask.AsTask);

            // then
            actualServiceDependencyException.Should().BeEquivalentTo(
                expectedServiceDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(serviceId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateServiceAsync(someService), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedServiceDependencyException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDatetimeOffset();
            Service randomService = CreateRandomService(randomDateTime);
            Service someService = randomService;
            someService.CreatedDate = someService.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageServiceException =
                new FailedServiceStorageException(databaseUpdateException);

            var expectedServiceDependencyException =
                new ServiceDependencyException(failedStorageServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(someService);

            ServiceDependencyException actualServiceDependencyException =
                await Assert.ThrowsAsync<ServiceDependencyException>(
                    modifyServiceTask.AsTask);

            // then
            actualServiceDependencyException.Should().BeEquivalentTo(expectedServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceDependencyException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Service randomService = CreateRandomService(randomDateTime);
            Service someService = randomService;
            someService.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedServiceException =
                new LockedServiceException(databaseUpdateConcurrencyException);

            var expectedServiceDependencyValidationException =
                new ServiceDependencyValidationException(lockedServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(someService);

            ServiceDependencyValidationException actualServiceDependencyValidationException =
                await Assert.ThrowsAsync<ServiceDependencyValidationException>(modifyServiceTask.AsTask);

            // then
            actualServiceDependencyValidationException.Should()
                .BeEquivalentTo(expectedServiceDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            var randomDateTime = GetRandomDateTime();
            Service randomService = CreateRandomService(randomDateTime);
            Service someService = randomService;
            someService.CreatedDate = someService.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedServiceServiceException =
                new FailedServiceServiceException(serviceException);

            var expectedServiceServiceException =
                new ServiceServiceException(failedServiceServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(someService);

            ServiceServiceException actualServiceServiceException =
                await Assert.ThrowsAsync<ServiceServiceException>(
                    modifyServiceTask.AsTask);

            // then
            actualServiceServiceException.Should().BeEquivalentTo(expectedServiceServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
