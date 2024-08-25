// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Addresss;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Addresss
{
    public partial class AddressServiceTests
    {
        [Fact]
        public async Task ShouldModifyAddressAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Address randomAddress = CreateRandomModifyAddress(randomDate);
            Address inputAddress = randomAddress;
            Address storageAddress = inputAddress.DeepClone();
            storageAddress.UpdatedDate = randomAddress.CreatedDate;
            Address updatedAddress = inputAddress;
            Address expectedAddress = updatedAddress.DeepClone();
            Guid addressId = inputAddress.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAddressByIdAsync(addressId))
                    .ReturnsAsync(storageAddress);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateAddressAsync(inputAddress))
                    .ReturnsAsync(updatedAddress);

            // when
            Address actualAddress =
               await this.addressService.ModifyAddressAsync(inputAddress);

            // then
            actualAddress.Should().BeEquivalentTo(expectedAddress);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAddressByIdAsync(addressId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAddressAsync(inputAddress), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
