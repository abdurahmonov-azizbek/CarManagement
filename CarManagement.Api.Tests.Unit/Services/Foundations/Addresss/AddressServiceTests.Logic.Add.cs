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
        public async Task ShouldAddAddressAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Address randomAddress = CreateRandomAddress(randomDate);
            Address inputAddress = randomAddress;
            Address persistedAddress = inputAddress;
            Address expectedAddress = persistedAddress.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertAddressAsync(inputAddress)).ReturnsAsync(persistedAddress);

            // when
            Address actualAddress = await this.addressService
                .AddAddressAsync(inputAddress);

            // then
            actualAddress.Should().BeEquivalentTo(expectedAddress);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAddressAsync(inputAddress), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}