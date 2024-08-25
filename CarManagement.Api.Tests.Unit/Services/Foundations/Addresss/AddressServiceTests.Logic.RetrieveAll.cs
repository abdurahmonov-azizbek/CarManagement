// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
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
        public void ShouldRetrieveAllAddresss()
        {
            //given
            IQueryable<Address> randomAddresss = CreateRandomAddresss();
            IQueryable<Address> storageAddresss = randomAddresss;
            IQueryable<Address> expectedAddresss = storageAddresss.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAddresss()).Returns(storageAddresss);

            //when
            IQueryable<Address> actualAddresss =
                this.addressService.RetrieveAllAddresss();

            //then
            actualAddresss.Should().BeEquivalentTo(expectedAddresss);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAddresss(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
