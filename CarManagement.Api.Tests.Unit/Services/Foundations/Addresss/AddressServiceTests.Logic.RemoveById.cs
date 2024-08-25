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
        public async Task ShouldRemoveAddressByIdAsync()
        {
            // given
            Guid randomAddressId = Guid.NewGuid();
            Guid inputAddressId = randomAddressId;
            Address randomAddress = CreateRandomAddress();
            Address storageAddress = randomAddress;
            Address expectedInputAddress = storageAddress;
            Address deletedAddress = expectedInputAddress;
            Address expectedAddress = deletedAddress.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAddressByIdAsync(inputAddressId))
                    .ReturnsAsync(storageAddress);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteAddressAsync(expectedInputAddress))
                    .ReturnsAsync(deletedAddress);

            // when
            Address actualAddress = await this.addressService
                .RemoveAddressByIdAsync(inputAddressId);

            // then
            actualAddress.Should().BeEquivalentTo(expectedAddress);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAddressByIdAsync(inputAddressId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteAddressAsync(expectedInputAddress), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
