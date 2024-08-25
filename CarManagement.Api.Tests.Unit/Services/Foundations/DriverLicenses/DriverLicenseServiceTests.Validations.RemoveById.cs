// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;
using CarManagement.Api.Models.DriverLicenses.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.DriverLicenses
{
    public partial class DriverLicenseServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidDriverLicenseId = Guid.Empty;

            var invalidDriverLicenseException = new InvalidDriverLicenseException();

            invalidDriverLicenseException.AddData(
                key: nameof(DriverLicense.Id),
                values: "Id is required");

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(invalidDriverLicenseException);

            // when
            ValueTask<DriverLicense> removeDriverLicenseByIdTask =
                this.driverLicenseService.RemoveDriverLicenseByIdAsync(invalidDriverLicenseId);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    removeDriverLicenseByIdTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDriverLicenseAsync(It.IsAny<DriverLicense>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfDriverLicenseIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomDriverLicenseId = Guid.NewGuid();
            Guid inputDriverLicenseId = randomDriverLicenseId;
            DriverLicense noDriverLicense = null;

            var notFoundDriverLicenseException =
                new NotFoundDriverLicenseException(inputDriverLicenseId);

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(notFoundDriverLicenseException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(inputDriverLicenseId)).ReturnsAsync(noDriverLicense);

            // when
            ValueTask<DriverLicense> removeDriverLicenseByIdTask =
                this.driverLicenseService.RemoveDriverLicenseByIdAsync(inputDriverLicenseId);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    removeDriverLicenseByIdTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
