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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            ServiceType randomServiceType = CreateRandomServiceType();
            ServiceType someServiceType = randomServiceType;
            Guid serviceTypeId = someServiceType.Id;
            SqlException sqlException = CreateSqlException();

            var failedServiceTypeStorageException =
                new FailedServiceTypeStorageException(sqlException);

            var expectedServiceTypeDependencyException =
                new ServiceTypeDependencyException(failedServiceTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(someServiceType);

            ServiceTypeDependencyException actualServiceTypeDependencyException =
                await Assert.ThrowsAsync<ServiceTypeDependencyException>(
                    modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeDependencyException.Should().BeEquivalentTo(
                expectedServiceTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(serviceTypeId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateServiceTypeAsync(someServiceType), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedServiceTypeDependencyException))), Times.Once);

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
            ServiceType randomServiceType = CreateRandomServiceType(randomDateTime);
            ServiceType someServiceType = randomServiceType;
            someServiceType.CreatedDate = someServiceType.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageServiceTypeException =
                new FailedServiceTypeStorageException(databaseUpdateException);

            var expectedServiceTypeDependencyException =
                new ServiceTypeDependencyException(failedStorageServiceTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(someServiceType);

            ServiceTypeDependencyException actualServiceTypeDependencyException =
                await Assert.ThrowsAsync<ServiceTypeDependencyException>(
                    modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeDependencyException.Should().BeEquivalentTo(expectedServiceTypeDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeDependencyException))), Times.Once);

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
            ServiceType randomServiceType = CreateRandomServiceType(randomDateTime);
            ServiceType someServiceType = randomServiceType;
            someServiceType.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedServiceTypeException =
                new LockedServiceTypeException(databaseUpdateConcurrencyException);

            var expectedServiceTypeDependencyValidationException =
                new ServiceTypeDependencyValidationException(lockedServiceTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(someServiceType);

            ServiceTypeDependencyValidationException actualServiceTypeDependencyValidationException =
                await Assert.ThrowsAsync<ServiceTypeDependencyValidationException>(modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeDependencyValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeDependencyValidationException))), Times.Once);

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
            ServiceType randomServiceType = CreateRandomServiceType(randomDateTime);
            ServiceType someServiceType = randomServiceType;
            someServiceType.CreatedDate = someServiceType.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedServiceTypeServiceException =
                new FailedServiceTypeServiceException(serviceException);

            var expectedServiceTypeServiceException =
                new ServiceTypeServiceException(failedServiceTypeServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(someServiceType);

            ServiceTypeServiceException actualServiceTypeServiceException =
                await Assert.ThrowsAsync<ServiceTypeServiceException>(
                    modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeServiceException.Should().BeEquivalentTo(expectedServiceTypeServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
