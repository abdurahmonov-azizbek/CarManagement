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
        public async Task ShouldRetrieveDriverLicenseByIdAsync()
        {
            //given
            Guid randomDriverLicenseId = Guid.NewGuid();
            Guid inputDriverLicenseId = randomDriverLicenseId;
            DriverLicense randomDriverLicense = CreateRandomDriverLicense();
            DriverLicense storageDriverLicense = randomDriverLicense;
            DriverLicense excpectedDriverLicense = randomDriverLicense.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(inputDriverLicenseId)).ReturnsAsync(storageDriverLicense);

            //when
            DriverLicense actuallDriverLicense = await this.driverLicenseService.RetrieveDriverLicenseByIdAsync(inputDriverLicenseId);

            //then
            actuallDriverLicense.Should().BeEquivalentTo(excpectedDriverLicense);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(inputDriverLicenseId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}