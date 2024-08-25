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
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            DriverLicense nullDriverLicense = null;
            var nullDriverLicenseException = new NullDriverLicenseException();

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(nullDriverLicenseException);

            // when
            ValueTask<DriverLicense> addDriverLicenseTask = this.driverLicenseService.AddDriverLicenseAsync(nullDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(addDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedDriverLicenseValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDriverLicenseAsync(It.IsAny<DriverLicense>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfJobIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            DriverLicense invalidDriverLicense = new DriverLicense()
            {
                FirstName = invalidText
            };

            var invalidDriverLicenseException = new InvalidDriverLicenseException();

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.Id),
					values: "Id is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.FirstName),
					values: "Text is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.LastName),
					values: "Text is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.MiddleName),
					values: "Text is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.AddressId),
					values: "Id is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.CategoryId),
					values: "Id is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.GivenLocation),
					values: "Text is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.Number),
					values: "Text is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.UserId),
					values: "Id is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.CreatedDate),
					values: "Date is required");

				invalidDriverLicenseException.AddData(
					key: nameof(DriverLicense.UpdatedDate),
					values: "Date is required");



            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(invalidDriverLicenseException);

            // when
            ValueTask<DriverLicense> addDriverLicenseTask = this.driverLicenseService.AddDriverLicenseAsync(invalidDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(addDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDriverLicenseAsync(It.IsAny<DriverLicense>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(randomDate);
            DriverLicense invalidDriverLicense = randomDriverLicense;
            invalidDriverLicense.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidDriverLicenseException = new InvalidDriverLicenseException();

            invalidDriverLicenseException.AddData(
                key: nameof(DriverLicense.CreatedDate),
                values: $"Date is not same as {nameof(DriverLicense.UpdatedDate)}");

            var expectedDriverLicenseValidationException = new DriverLicenseValidationException(invalidDriverLicenseException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<DriverLicense> addDriverLicenseTask = this.driverLicenseService.AddDriverLicenseAsync(invalidDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(addDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should().BeEquivalentTo(expectedDriverLicenseValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDriverLicenseAsync(It.IsAny<DriverLicense>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinutes))]
        public async Task ShouldThrowValidationExceptionIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidMinutes)
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            DateTimeOffset invalidDateTime = randomDate.AddMinutes(invalidMinutes);
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(invalidDateTime);
            DriverLicense invalidDriverLicense = randomDriverLicense;
            var invalidDriverLicenseException = new InvalidDriverLicenseException();

            invalidDriverLicenseException.AddData(
                key: nameof(DriverLicense.CreatedDate),
                values: "Date is not recent");

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(invalidDriverLicenseException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<DriverLicense> addDriverLicenseTask = this.driverLicenseService.AddDriverLicenseAsync(invalidDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(addDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should().
                BeEquivalentTo(expectedDriverLicenseValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDriverLicenseAsync(It.IsAny<DriverLicense>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}