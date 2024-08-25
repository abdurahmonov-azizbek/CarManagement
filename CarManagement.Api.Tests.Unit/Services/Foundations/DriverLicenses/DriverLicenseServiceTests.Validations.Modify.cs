// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;
using CarManagement.Api.Models.DriverLicenses.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.DriverLicenses
{
    public partial class DriverLicenseServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfDriverLicenseIsNullAndLogItAsync()
        {
            // given
            DriverLicense nullDriverLicense = null;
            var nullDriverLicenseException = new NullDriverLicenseException();

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(nullDriverLicenseException);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask = this.driverLicenseService.ModifyDriverLicenseAsync(nullDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfDriverLicenseIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            DriverLicense invalidDriverLicense = new DriverLicense
            {
                FirstName = invalidString
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
                values: new[]
                    {
                        "Date is required",
                        "Date is not recent",
                        $"Date is the same as {nameof(DriverLicense.CreatedDate)}"
                    }
                );

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(invalidDriverLicenseException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask = this.driverLicenseService.ModifyDriverLicenseAsync(invalidDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(randomDateTime);
            DriverLicense invalidDriverLicense = randomDriverLicense;
            var invalidDriverLicenseException = new InvalidDriverLicenseException();

            invalidDriverLicenseException.AddData(
                key: nameof(DriverLicense.UpdatedDate),
                values: $"Date is the same as {nameof(DriverLicense.CreatedDate)}");

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(invalidDriverLicenseException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(invalidDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                 await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(invalidDriverLicense.Id), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(dateTime);
            DriverLicense inputDriverLicense = randomDriverLicense;
            inputDriverLicense.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidDriverLicenseException = new InvalidDriverLicenseException();

            invalidDriverLicenseException.AddData(
                key: nameof(DriverLicense.UpdatedDate),
                values: "Date is not recent");

            var expectedDriverLicenseValidatonException =
                new DriverLicenseValidationException(invalidDriverLicenseException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(inputDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidatonException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfDriverLicenseDoesNotExistAndLogItAsync()
        {
            // given
            int negativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            DriverLicense randomDriverLicense = CreateRandomDriverLicense(dateTime);
            DriverLicense nonExistDriverLicense = randomDriverLicense;
            nonExistDriverLicense.CreatedDate = dateTime.AddMinutes(negativeMinutes);
            DriverLicense nullDriverLicense = null;

            var notFoundDriverLicenseException = new NotFoundDriverLicenseException(nonExistDriverLicense.Id);

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(notFoundDriverLicenseException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(nonExistDriverLicense.Id))
                    .ReturnsAsync(nullDriverLicense);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(nonExistDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(
                    modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(nonExistDriverLicense.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            DriverLicense randomDriverLicense = CreateRandomModifyDriverLicense(randomDateTime);
            DriverLicense invalidDriverLicense = randomDriverLicense.DeepClone();
            DriverLicense storageDriverLicense = invalidDriverLicense.DeepClone();
            storageDriverLicense.CreatedDate = storageDriverLicense.CreatedDate.AddMinutes(randomMinutes);
            storageDriverLicense.UpdatedDate = storageDriverLicense.UpdatedDate.AddMinutes(randomMinutes);
            var invalidDriverLicenseException = new InvalidDriverLicenseException();
            Guid driverLicenseId = invalidDriverLicense.Id;

            invalidDriverLicenseException.AddData(
                key: nameof(DriverLicense.CreatedDate),
                values: $"Date is not same as {nameof(DriverLicense.CreatedDate)}");

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(invalidDriverLicenseException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(driverLicenseId)).ReturnsAsync(storageDriverLicense);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(invalidDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(invalidDriverLicense.Id), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            DriverLicense randomDriverLicense = CreateRandomModifyDriverLicense(randomDateTime);
            DriverLicense invalidDriverLicense = randomDriverLicense;
            DriverLicense storageDriverLicense = randomDriverLicense.DeepClone();
            invalidDriverLicense.UpdatedDate = storageDriverLicense.UpdatedDate;
            Guid driverLicenseId = invalidDriverLicense.Id;
            var invalidDriverLicenseException = new InvalidDriverLicenseException();

            invalidDriverLicenseException.AddData(
                key: nameof(DriverLicense.UpdatedDate),
                values: $"Date is the same as {nameof(DriverLicense.UpdatedDate)}");

            var expectedDriverLicenseValidationException =
                new DriverLicenseValidationException(invalidDriverLicenseException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDriverLicenseByIdAsync(invalidDriverLicense.Id)).ReturnsAsync(storageDriverLicense);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<DriverLicense> modifyDriverLicenseTask =
                this.driverLicenseService.ModifyDriverLicenseAsync(invalidDriverLicense);

            DriverLicenseValidationException actualDriverLicenseValidationException =
                await Assert.ThrowsAsync<DriverLicenseValidationException>(modifyDriverLicenseTask.AsTask);

            // then
            actualDriverLicenseValidationException.Should()
                .BeEquivalentTo(expectedDriverLicenseValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDriverLicenseByIdAsync(driverLicenseId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDriverLicenseValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
