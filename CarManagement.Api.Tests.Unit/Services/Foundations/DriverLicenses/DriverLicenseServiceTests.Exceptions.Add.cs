// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;
using CarManagement.Api.Models.DriverLicenses.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            DriverLicense someDriverLicense = CreateRandomDriverLicense();
            Guid driverLicenseId = someDriverLicense.Id;
            SqlException sqlException = CreateSqlException();

            FailedDriverLicenseStorageException failedDriverLicenseStorageException =
                new FailedDriverLicenseStorageException(sqlException);

            DriverLicenseDependencyException expectedDriverLicenseDependencyException =
                new DriverLicenseDependencyException(failedDriverLicenseStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<DriverLicense> addDriverLicenseTask = this.driverLicenseService
                .AddDriverLicenseAsync(someDriverLicense);

            DriverLicenseDependencyException actualDriverLicenseDependencyException =
                await Assert.ThrowsAsync<DriverLicenseDependencyException>(addDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseDependencyException.Should().BeEquivalentTo(expectedDriverLicenseDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedDriverLicenseDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            DriverLicense someDriverLicense = CreateRandomDriverLicense();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsDriverLicenseException =
                new AlreadyExistsDriverLicenseException(duplicateKeyException);

            var expectedDriverLicenseDependencyValidationException =
                new DriverLicenseDependencyValidationException(alreadyExistsDriverLicenseException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<DriverLicense> addDriverLicenseTask = this.driverLicenseService.AddDriverLicenseAsync(someDriverLicense);

            DriverLicenseDependencyValidationException actualDriverLicenseDependencyValidationException =
                await Assert.ThrowsAsync<DriverLicenseDependencyValidationException>(
                    addDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseDependencyValidationException.Should().BeEquivalentTo(
                expectedDriverLicenseDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            DriverLicense someDriverLicense = CreateRandomDriverLicense();
            var dbUpdateException = new DbUpdateException();

            var failedDriverLicenseStorageException = new FailedDriverLicenseStorageException(dbUpdateException);

            var expectedDriverLicenseDependencyException =
                new DriverLicenseDependencyException(failedDriverLicenseStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<DriverLicense> addDriverLicenseTask = this.driverLicenseService.AddDriverLicenseAsync(someDriverLicense);

            DriverLicenseDependencyException actualDriverLicenseDependencyException =
                 await Assert.ThrowsAsync<DriverLicenseDependencyException>(addDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseDependencyException.Should()
                .BeEquivalentTo(expectedDriverLicenseDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedDriverLicenseDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDriverLicenseAsync(It.IsAny<DriverLicense>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            DriverLicense someDriverLicense = CreateRandomDriverLicense();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedDriverLicenseStorageException =
                new FailedDriverLicenseStorageException(dbUpdateException);

            var expectedDriverLicenseDependencyException =
                new DriverLicenseDependencyException(failedDriverLicenseStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<DriverLicense> addDriverLicenseTask =
                this.driverLicenseService.AddDriverLicenseAsync(someDriverLicense);

            DriverLicenseDependencyException actualDriverLicenseDependencyException =
                await Assert.ThrowsAsync<DriverLicenseDependencyException>(addDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseDependencyException.Should().BeEquivalentTo(expectedDriverLicenseDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            DriverLicense someDriverLicense = CreateRandomDriverLicense();
            var serviceException = new Exception();
            var failedDriverLicenseException = new FailedDriverLicenseServiceException(serviceException);

            var expectedDriverLicenseServiceExceptions =
                new DriverLicenseServiceException(failedDriverLicenseException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<DriverLicense> addDriverLicenseTask = this.driverLicenseService.AddDriverLicenseAsync(someDriverLicense);

            DriverLicenseServiceException actualDriverLicenseServiceException =
                await Assert.ThrowsAsync<DriverLicenseServiceException>(addDriverLicenseTask.AsTask);

            //then
            actualDriverLicenseServiceException.Should().BeEquivalentTo(
                expectedDriverLicenseServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}