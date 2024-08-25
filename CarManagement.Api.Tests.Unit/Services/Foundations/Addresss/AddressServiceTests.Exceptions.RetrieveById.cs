// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Addresss;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedAddressStorageException =
                new FailedAddressStorageException(sqlException);

            var expectedAddressDependencyException =
                new AddressDependencyException(failedAddressStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAddressByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<Address> retrieveAddressByIdTask =
                this.addressService.RetrieveAddressByIdAsync(someId);

            AddressDependencyException actualAddressDependencyexception =
                await Assert.ThrowsAsync<AddressDependencyException>(
                    retrieveAddressByIdTask.AsTask);

            //then
            actualAddressDependencyexception.Should().BeEquivalentTo(
                expectedAddressDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAddressByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedAddressDependencyException))), Times.Once);

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

            var failedAddressServiceException =
                new FailedAddressServiceException(serviceException);

            var expectedAddressServiceException =
                new AddressServiceException(failedAddressServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAddressByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<Address> retrieveAddressByIdTask =
                this.addressService.RetrieveAddressByIdAsync(someId);

            AddressServiceException actualAddressServiceException =
                await Assert.ThrowsAsync<AddressServiceException>(retrieveAddressByIdTask.AsTask);

            //then
            actualAddressServiceException.Should().BeEquivalentTo(expectedAddressServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAddressByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}