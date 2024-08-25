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
        public async Task ShouldAddDriverLicenseAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(randomDate);
            DriverLicense inputDriverLicense = randomDriverLicense;
            DriverLicense persistedDriverLicense = inputDriverLicense;
            DriverLicense expectedDriverLicense = persistedDriverLicense.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertDriverLicenseAsync(inputDriverLicense)).ReturnsAsync(persistedDriverLicense);

            // when
            DriverLicense actualDriverLicense = await this.driverLicenseService
                .AddDriverLicenseAsync(inputDriverLicense);

            // then
            actualDriverLicense.Should().BeEquivalentTo(expectedDriverLicense);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDriverLicenseAsync(inputDriverLicense), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}