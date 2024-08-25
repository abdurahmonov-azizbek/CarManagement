// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldRemoveDriverLicenseByIdAsync()
        {
            // given
            Guid randomDriverLicenseId = Guid.NewGuid();
            Guid inputDriverLicenseId = randomDriverLicenseId;
            DriverLicense randomDriverLicense = CreateRandomDriverLicense();
            DriverLicense storageDriverLicense = randomDriverLicense;
            DriverLicense expectedInputDriverLicense = storageDriverLicense;
            DriverLicense deletedDriverLicense = expectedInputDriverLicense;
            DriverLicense expectedDriverLicense = deletedDriverLicense.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(inputDriverLicenseId))
                    .ReturnsAsync(storageDriverLicense);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteDriverLicenseAsync(expectedInputDriverLicense))
                    .ReturnsAsync(deletedDriverLicense);

            // when
            DriverLicense actualDriverLicense = await this.driverLicenseService
                .RemoveDriverLicenseByIdAsync(inputDriverLicenseId);

            // then
            actualDriverLicense.Should().BeEquivalentTo(expectedDriverLicense);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(inputDriverLicenseId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDriverLicenseAsync(expectedInputDriverLicense), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
