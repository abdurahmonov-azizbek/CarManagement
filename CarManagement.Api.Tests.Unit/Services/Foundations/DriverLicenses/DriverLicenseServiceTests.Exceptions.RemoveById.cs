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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someDriverLicenseId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedDriverLicenseException =
                new LockedDriverLicenseException(databaseUpdateConcurrencyException);

            var expectedDriverLicenseDependencyValidationException =
                new DriverLicenseDependencyValidationException(lockedDriverLicenseException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<DriverLicense> removeDriverLicenseByIdTask =
               this.driverLicenseService.RemoveDriverLicenseByIdAsync(someDriverLicenseId);

            DriverLicenseDependencyValidationException actualDriverLicenseDependencyValidationException =
                await Assert.ThrowsAsync<DriverLicenseDependencyValidationException>(
                    removeDriverLicenseByIdTask.AsTask);

            // then
            actualDriverLicenseDependencyValidationException.Should().BeEquivalentTo(
               expectedDriverLicenseDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDriverLicenseAsync(It.IsAny<DriverLicense>()), Times.Never);

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

            var failedDriverLicenseStorageException =
                new FailedDriverLicenseStorageException(sqlException);

            var expectedDriverLicenseDependencyException =
                new DriverLicenseDependencyException(failedDriverLicenseStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<DriverLicense> removeDriverLicenseTask =
                this.driverLicenseService.RemoveDriverLicenseByIdAsync(someId);

            DriverLicenseDependencyException actualDriverLicenseDependencyException =
                await Assert.ThrowsAsync<DriverLicenseDependencyException>(
                    removeDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseDependencyException.Should().BeEquivalentTo(expectedDriverLicenseDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedDriverLicenseDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someDriverLicenseId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedDriverLicenseServiceException =
                new FailedDriverLicenseServiceException(serviceException);

            var expectedDriverLicenseServiceException =
                new DriverLicenseServiceException(failedDriverLicenseServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(someDriverLicenseId))
                    .Throws(serviceException);

            // when
            ValueTask<DriverLicense> removeDriverLicenseByIdTask =
                this.driverLicenseService.RemoveDriverLicenseByIdAsync(someDriverLicenseId);

            DriverLicenseServiceException actualDriverLicenseServiceException =
                await Assert.ThrowsAsync<DriverLicenseServiceException>(
                    removeDriverLicenseByIdTask.AsTask);

            // then
            actualDriverLicenseServiceException.Should().BeEquivalentTo(expectedDriverLicenseServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}