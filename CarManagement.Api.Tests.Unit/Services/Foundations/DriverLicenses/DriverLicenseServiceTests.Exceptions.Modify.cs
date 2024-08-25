// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;
using CarManagement.Api.Models.DriverLicenses.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.DriverLicenses
{
    public partial class DriverLicenseServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            DriverLicense randomDriverLicense = CreateRandomDriverLicense();
            DriverLicense someDriverLicense = randomDriverLicense;
            Guid driverLicenseId = someDriverLicense.Id;
            SqlException sqlException = CreateSqlException();

            var failedDriverLicenseStorageException =
                new FailedDriverLicenseStorageException(sqlException);

            var expectedDriverLicenseDependencyException =
                new DriverLicenseDependencyException(failedDriverLicenseStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(someDriverLicense);

            DriverLicenseDependencyException actualDriverLicenseDependencyException =
                await Assert.ThrowsAsync<DriverLicenseDependencyException>(
                    modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseDependencyException.Should().BeEquivalentTo(
                expectedDriverLicenseDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(driverLicenseId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDriverLicenseAsync(someDriverLicense), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedDriverLicenseDependencyException))), Times.Once);

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
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(randomDateTime);
            DriverLicense someDriverLicense = randomDriverLicense;
            someDriverLicense.CreatedDate = someDriverLicense.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageDriverLicenseException =
                new FailedDriverLicenseStorageException(databaseUpdateException);

            var expectedDriverLicenseDependencyException =
                new DriverLicenseDependencyException(failedStorageDriverLicenseException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(someDriverLicense);

            DriverLicenseDependencyException actualDriverLicenseDependencyException =
                await Assert.ThrowsAsync<DriverLicenseDependencyException>(
                    modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseDependencyException.Should().BeEquivalentTo(expectedDriverLicenseDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseDependencyException))), Times.Once);

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
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(randomDateTime);
            DriverLicense someDriverLicense = randomDriverLicense;
            someDriverLicense.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedDriverLicenseException =
                new LockedDriverLicenseException(databaseUpdateConcurrencyException);

            var expectedDriverLicenseDependencyValidationException =
                new DriverLicenseDependencyValidationException(lockedDriverLicenseException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(someDriverLicense);

            DriverLicenseDependencyValidationException actualDriverLicenseDependencyValidationException =
                await Assert.ThrowsAsync<DriverLicenseDependencyValidationException>(modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseDependencyValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseDependencyValidationException))), Times.Once);

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
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(randomDateTime);
            DriverLicense someDriverLicense = randomDriverLicense;
            someDriverLicense.CreatedDate = someDriverLicense.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedDriverLicenseServiceException =
                new FailedDriverLicenseServiceException(serviceException);

            var expectedDriverLicenseServiceException =
                new DriverLicenseServiceException(failedDriverLicenseServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(someDriverLicense);

            DriverLicenseServiceException actualDriverLicenseServiceException =
                await Assert.ThrowsAsync<DriverLicenseServiceException>(
                    modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseServiceException.Should().BeEquivalentTo(expectedDriverLicenseServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
