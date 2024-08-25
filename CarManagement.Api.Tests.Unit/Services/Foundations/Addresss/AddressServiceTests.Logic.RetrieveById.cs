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
        public async Task ShouldRetrieveAddressByIdAsync()
        {
            //given
            Guid randomAddressId = Guid.NewGuid();
            Guid inputAddressId = randomAddressId;
            Address randomAddress = CreateRandomAddress();
            Address storageAddress = randomAddress;
            Address excpectedAddress = randomAddress.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAddressByIdAsync(inputAddressId)).ReturnsAsync(storageAddress);

            //when
            Address actuallAddress = await this.addressService.RetrieveAddressByIdAsync(inputAddressId);

            //then
            actuallAddress.Should().BeEquivalentTo(excpectedAddress);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAddressByIdAsync(inputAddressId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}