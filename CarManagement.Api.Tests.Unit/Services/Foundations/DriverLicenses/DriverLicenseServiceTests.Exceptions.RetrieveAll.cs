// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedDriverLicenseStorageException(sqlException);

            var expectedDriverLicenseDependencyException =
                new DriverLicenseDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDriverLicenses()).Throws(sqlException);

            //when
            Action retrieveAllDriverLicensesAction = () =>
                this.driverLicenseService.RetrieveAllDriverLicenses();

            DriverLicenseDependencyException actualDriverLicenseDependencyException =
                Assert.Throws<DriverLicenseDependencyException>(retrieveAllDriverLicensesAction);

            //then
            actualDriverLicenseDependencyException.Should().BeEquivalentTo(
                expectedDriverLicenseDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDriverLicenses(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedDriverLicenseDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedDriverLicenseServiceException =
                new FailedDriverLicenseServiceException(serviceException);

            var expectedDriverLicenseServiceException =
                new DriverLicenseServiceException(failedDriverLicenseServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDriverLicenses()).Throws(serviceException);

            // when
            Action retrieveAllDriverLicensesAction = () =>
                this.driverLicenseService.RetrieveAllDriverLicenses();

            DriverLicenseServiceException actualDriverLicenseServiceException =
                Assert.Throws<DriverLicenseServiceException>(retrieveAllDriverLicensesAction);

            // then
            actualDriverLicenseServiceException.Should().BeEquivalentTo(expectedDriverLicenseServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDriverLicenses(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}