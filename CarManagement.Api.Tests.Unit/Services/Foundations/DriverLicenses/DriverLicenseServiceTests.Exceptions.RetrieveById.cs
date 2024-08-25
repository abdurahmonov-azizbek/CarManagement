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
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.DriverLicenses
{
    public partial class DriverLicenseServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedDriverLicenseStorageException =
                new FailedDriverLicenseStorageException(sqlException);

            var expectedDriverLicenseDependencyException =
                new DriverLicenseDependencyException(failedDriverLicenseStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<DriverLicense> retrieveDriverLicenseByIdTask =
                this.driverLicenseService.RetrieveDriverLicenseByIdAsync(someId);

            DriverLicenseDependencyException actualDriverLicenseDependencyexception =
                await Assert.ThrowsAsync<DriverLicenseDependencyException>(
                    retrieveDriverLicenseByIdTask.AsTask);

            //then
            actualDriverLicenseDependencyexception.Should().BeEquivalentTo(
                expectedDriverLicenseDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedDriverLicenseServiceException =
                new FailedDriverLicenseServiceException(serviceException);

            var expectedDriverLicenseServiceException =
                new DriverLicenseServiceException(failedDriverLicenseServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<DriverLicense> retrieveDriverLicenseByIdTask =
                this.driverLicenseService.RetrieveDriverLicenseByIdAsync(someId);

            DriverLicenseServiceException actualDriverLicenseServiceException =
                await Assert.ThrowsAsync<DriverLicenseServiceException>(retrieveDriverLicenseByIdTask.AsTask);

            //then
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