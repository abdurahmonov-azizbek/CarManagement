// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Addresss.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Addresss
{
    public partial class AddressServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedAddressStorageException(sqlException);

            var expectedAddressDependencyException =
                new AddressDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAddresss()).Throws(sqlException);

            //when
            Action retrieveAllAddresssAction = () =>
                this.addressService.RetrieveAllAddresss();

            AddressDependencyException actualAddressDependencyException =
                Assert.Throws<AddressDependencyException>(retrieveAllAddresssAction);

            //then
            actualAddressDependencyException.Should().BeEquivalentTo(
                expectedAddressDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAddresss(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedAddressDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedAddressServiceException =
                new FailedAddressServiceException(serviceException);

            var expectedAddressServiceException =
                new AddressServiceException(failedAddressServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAddresss()).Throws(serviceException);

            // when
            Action retrieveAllAddresssAction = () =>
                this.addressService.RetrieveAllAddresss();

            AddressServiceException actualAddressServiceException =
                Assert.Throws<AddressServiceException>(retrieveAllAddresssAction);

            // then
            actualAddressServiceException.Should().BeEquivalentTo(expectedAddressServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAddresss(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}