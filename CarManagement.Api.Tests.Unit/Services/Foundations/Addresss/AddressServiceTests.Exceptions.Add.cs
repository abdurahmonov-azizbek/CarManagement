// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Addresss;
using CarManagement.Api.Models.Addresss.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            Address someAddress = CreateRandomAddress();
            Guid addressId = someAddress.Id;
            SqlException sqlException = CreateSqlException();

            FailedAddressStorageException failedAddressStorageException =
                new FailedAddressStorageException(sqlException);

            AddressDependencyException expectedAddressDependencyException =
                new AddressDependencyException(failedAddressStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Address> addAddressTask = this.addressService
                .AddAddressAsync(someAddress);

            AddressDependencyException actualAddressDependencyException =
                await Assert.ThrowsAsync<AddressDependencyException>(addAddressTask.AsTask);

            // then
            actualAddressDependencyException.Should().BeEquivalentTo(expectedAddressDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedAddressDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            Address someAddress = CreateRandomAddress();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsAddressException =
                new AlreadyExistsAddressException(duplicateKeyException);

            var expectedAddressDependencyValidationException =
                new AddressDependencyValidationException(alreadyExistsAddressException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<Address> addAddressTask = this.addressService.AddAddressAsync(someAddress);

            AddressDependencyValidationException actualAddressDependencyValidationException =
                await Assert.ThrowsAsync<AddressDependencyValidationException>(
                    addAddressTask.AsTask);

            // then
            actualAddressDependencyValidationException.Should().BeEquivalentTo(
                expectedAddressDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            Address someAddress = CreateRandomAddress();
            var dbUpdateException = new DbUpdateException();

            var failedAddressStorageException = new FailedAddressStorageException(dbUpdateException);

            var expectedAddressDependencyException =
                new AddressDependencyException(failedAddressStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Address> addAddressTask = this.addressService.AddAddressAsync(someAddress);

            AddressDependencyException actualAddressDependencyException =
                 await Assert.ThrowsAsync<AddressDependencyException>(addAddressTask.AsTask);

            // then
            actualAddressDependencyException.Should()
                .BeEquivalentTo(expectedAddressDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedAddressDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAddressAsync(It.IsAny<Address>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            Address someAddress = CreateRandomAddress();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedAddressStorageException =
                new FailedAddressStorageException(dbUpdateException);

            var expectedAddressDependencyException =
                new AddressDependencyException(failedAddressStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<Address> addAddressTask =
                this.addressService.AddAddressAsync(someAddress);

            AddressDependencyException actualAddressDependencyException =
                await Assert.ThrowsAsync<AddressDependencyException>(addAddressTask.AsTask);

            // then
            actualAddressDependencyException.Should().BeEquivalentTo(expectedAddressDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            Address someAddress = CreateRandomAddress();
            var serviceException = new Exception();
            var failedAddressException = new FailedAddressServiceException(serviceException);

            var expectedAddressServiceExceptions =
                new AddressServiceException(failedAddressException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<Address> addAddressTask = this.addressService.AddAddressAsync(someAddress);

            AddressServiceException actualAddressServiceException =
                await Assert.ThrowsAsync<AddressServiceException>(addAddressTask.AsTask);

            //then
            actualAddressServiceException.Should().BeEquivalentTo(
                expectedAddressServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}