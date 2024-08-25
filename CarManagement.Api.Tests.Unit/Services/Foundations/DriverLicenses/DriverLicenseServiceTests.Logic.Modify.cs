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
        public async Task ShouldModifyDriverLicenseAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            DriverLicense randomDriverLicense = CreateRandomModifyDriverLicense(randomDate);
            DriverLicense inputDriverLicense = randomDriverLicense;
            DriverLicense storageDriverLicense = inputDriverLicense.DeepClone();
            storageDriverLicense.UpdatedDate = randomDriverLicense.CreatedDate;
            DriverLicense updatedDriverLicense = inputDriverLicense;
            DriverLicense expectedDriverLicense = updatedDriverLicense.DeepClone();
            Guid driverLicenseId = inputDriverLicense.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(driverLicenseId))
                    .ReturnsAsync(storageDriverLicense);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateDriverLicenseAsync(inputDriverLicense))
                    .ReturnsAsync(updatedDriverLicense);

            // when
            DriverLicense actualDriverLicense =
               await this.driverLicenseService.ModifyDriverLicenseAsync(inputDriverLicense);

            // then
            actualDriverLicense.Should().BeEquivalentTo(expectedDriverLicense);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(driverLicenseId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDriverLicenseAsync(inputDriverLicense), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
