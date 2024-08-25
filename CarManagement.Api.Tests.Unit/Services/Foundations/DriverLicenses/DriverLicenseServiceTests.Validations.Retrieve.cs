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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalidDriverLicenseId = Guid.Empty;
            var invalidDriverLicenseException = new InvalidDriverLicenseException();

            invalidDriverLicenseException.AddData(
                key: nameof(DriverLicense.Id),
                values: "Id is required");

            var excpectedDriverLicenseValidationException = new
                DriverLicenseValidationException(invalidDriverLicenseException);

            //when
            ValueTask<DriverLicense> retrieveDriverLicenseByIdTask =
                this.driverLicenseService.RetrieveDriverLicenseByIdAsync(invalidDriverLicenseId);

            DriverLicenseValidationException actuallDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    retrieveDriverLicenseByIdTask.AsTask);

            //then
            actuallDriverLicenseValidationException.Should()
                .BeEquivalentTo(excpectedDriverLicenseValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfDriverLicenseIsNotFoundAndLogItAsync()
        {
            Guid someDriverLicenseId = Guid.NewGuid();
            DriverLicense noDriverLicense = null;

            var notFoundDriverLicenseException =
                new NotFoundDriverLicenseException(someDriverLicenseId);

            var excpectedDriverLicenseValidationException =
                new DriverLicenseValidationException(notFoundDriverLicenseException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noDriverLicense);

            //when 
            ValueTask<DriverLicense> retrieveDriverLicenseByIdTask =
                this.driverLicenseService.RetrieveDriverLicenseByIdAsync(someDriverLicenseId);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    retrieveDriverLicenseByIdTask.AsTask);

            //then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(excpectedDriverLicenseValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
