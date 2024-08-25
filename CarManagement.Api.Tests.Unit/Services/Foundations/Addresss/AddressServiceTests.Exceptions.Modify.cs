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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            Address randomAddress = CreateRandomAddress();
            Address someAddress = randomAddress;
            Guid addressId = someAddress.Id;
            SqlException sqlException = CreateSqlException();

            var failedAddressStorageException =
                new FailedAddressStorageException(sqlException);

            var expectedAddressDependencyException =
                new AddressDependencyException(failedAddressStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<Address> modifyAddressTask =
                this.addressService.ModifyAddressAsync(someAddress);

            AddressDependencyException actualAddressDependencyException =
                await Assert.ThrowsAsync<AddressDependencyException>(
                    modifyAddressTask.AsTask);

            // then
            actualAddressDependencyException.Should().BeEquivalentTo(
                expectedAddressDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAddressByIdAsync(addressId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAddressAsync(someAddress), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedAddressDependencyException))), Times.Once);

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
            Address randomAddress = CreateRandomAddress(randomDateTime);
            Address someAddress = randomAddress;
            someAddress.CreatedDate = someAddress.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageAddressException =
                new FailedAddressStorageException(databaseUpdateException);

            var expectedAddressDependencyException =
                new AddressDependencyException(failedStorageAddressException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Address> modifyAddressTask =
                this.addressService.ModifyAddressAsync(someAddress);

            AddressDependencyException actualAddressDependencyException =
                await Assert.ThrowsAsync<AddressDependencyException>(
                    modifyAddressTask.AsTask);

            // then
            actualAddressDependencyException.Should().BeEquivalentTo(expectedAddressDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressDependencyException))), Times.Once);

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
            Address randomAddress = CreateRandomAddress(randomDateTime);
            Address someAddress = randomAddress;
            someAddress.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedAddressException =
                new LockedAddressException(databaseUpdateConcurrencyException);

            var expectedAddressDependencyValidationException =
                new AddressDependencyValidationException(lockedAddressException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Address> modifyAddressTask =
                this.addressService.ModifyAddressAsync(someAddress);

            AddressDependencyValidationException actualAddressDependencyValidationException =
                await Assert.ThrowsAsync<AddressDependencyValidationException>(modifyAddressTask.AsTask);

            // then
            actualAddressDependencyValidationException.Should()
                .BeEquivalentTo(expectedAddressDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressDependencyValidationException))), Times.Once);

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
            Address randomAddress = CreateRandomAddress(randomDateTime);
            Address someAddress = randomAddress;
            someAddress.CreatedDate = someAddress.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedAddressServiceException =
                new FailedAddressServiceException(serviceException);

            var expectedAddressServiceException =
                new AddressServiceException(failedAddressServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Address> modifyAddressTask =
                this.addressService.ModifyAddressAsync(someAddress);

            AddressServiceException actualAddressServiceException =
                await Assert.ThrowsAsync<AddressServiceException>(
                    modifyAddressTask.AsTask);

            // then
            actualAddressServiceException.Should().BeEquivalentTo(expectedAddressServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAddressServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
