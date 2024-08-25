// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
using CarManagement.Api.Models.DriverLicenses;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.DriverLicenses
{
    public partial class DriverLicenseServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllDriverLicenses()
        {
            //given
            IQueryable<DriverLicense> randomDriverLicenses = CreateRandomDriverLicenses();
            IQueryable<DriverLicense> storageDriverLicenses = randomDriverLicenses;
            IQueryable<DriverLicense> expectedDriverLicenses = storageDriverLicenses.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDriverLicenses()).Returns(storageDriverLicenses);

            //when
            IQueryable<DriverLicense> actualDriverLicenses =
                this.driverLicenseService.RetrieveAllDriverLicenses();

            //then
            actualDriverLicenses.Should().BeEquivalentTo(expectedDriverLicenses);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDriverLicenses(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
