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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Addresss
{
    public partial class AddressServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someAddressId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedAddressException =
                new LockedAddressException(databaseUpdateConcurrencyException);

            var expectedAddressDependencyValidationException =
                new AddressDependencyValidationException(lockedAddressException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAddressByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Address> removeAddressByIdTask =
               this.addressService.RemoveAddressByIdAsync(someAddressId);

            AddressDependencyValidationException actualAddressDependencyValidationException =
                await Assert.ThrowsAsync<AddressDependencyValidationException>(
                    removeAddressByIdTask.AsTask);

            // then
            actualAddressDependencyValidationException.Should().BeEquivalentTo(
               expectedAddressDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAddressByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteAddressAsync(It.IsAny<Address>()), Times.Never);

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

            var failedAddressStorageException =
                new FailedAddressStorageException(sqlException);

            var expectedAddressDependencyException =
                new AddressDependencyException(failedAddressStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAddressByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<Address> removeAddressTask =
                this.addressService.RemoveAddressByIdAsync(someId);

            AddressDependencyException actualAddressDependencyException =
                await Assert.ThrowsAsync<AddressDependencyException>(
                    removeAddressTask.AsTask);

            // then
            actualAddressDependencyException.Should().BeEquivalentTo(expectedAddressDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someAddressId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedAddressServiceException =
                new FailedAddressServiceException(serviceException);

            var expectedAddressServiceException =
                new AddressServiceException(failedAddressServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAddressByIdAsync(someAddressId))
                    .Throws(serviceException);

            // when
            ValueTask<Address> removeAddressByIdTask =
                this.addressService.RemoveAddressByIdAsync(someAddressId);

            AddressServiceException actualAddressServiceException =
                await Assert.ThrowsAsync<AddressServiceException>(
                    removeAddressByIdTask.AsTask);

            // then
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