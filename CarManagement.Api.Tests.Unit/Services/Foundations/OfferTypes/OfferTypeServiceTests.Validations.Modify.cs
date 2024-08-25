// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.OfferTypes
{
    public partial class OfferTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfOfferTypeIsNullAndLogItAsync()
        {
            // given
            OfferType nullOfferType = null;
            var nullOfferTypeException = new NullOfferTypeException();

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(nullOfferTypeException);

            // when
            ValueTask<OfferType> modifyOfferTypeTask = this.offerTypeService.ModifyOfferTypeAsync(nullOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(
                    modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfOfferTypeIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            OfferType invalidOfferType = new OfferType
            {
                Name = invalidString
            };

            var invalidOfferTypeException = new InvalidOfferTypeException();

				invalidOfferTypeException.AddData(
					key: nameof(OfferType.Id),
					values: "Id is required");

				invalidOfferTypeException.AddData(
					key: nameof(OfferType.Name),
					values: "Text is required");



            invalidOfferTypeException.AddData(
                key: nameof(OfferType.CreatedDate),
                values: "Date is required");

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.UpdatedDate),
                values: new[]
                    {
                        "Date is required",
                        "Date is not recent",
                        $"Date is the same as {nameof(OfferType.CreatedDate)}"
                    }
                );

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(invalidOfferTypeException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<OfferType> modifyOfferTypeTask = this.offerTypeService.ModifyOfferTypeAsync(invalidOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(
                    modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            OfferType randomOfferType = CreateRandomOfferType(randomDateTime);
            OfferType invalidOfferType = randomOfferType;
            var invalidOfferTypeException = new InvalidOfferTypeException();

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.UpdatedDate),
                values: $"Date is the same as {nameof(OfferType.CreatedDate)}");

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(invalidOfferTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(invalidOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                 await Assert.ThrowsAsync<OfferTypeValidationException>(
                    modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(invalidOfferType.Id), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

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
            OfferType randomOfferType = CreateRandomOfferType(dateTime);
            OfferType inputOfferType = randomOfferType;
            inputOfferType.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidOfferTypeException = new InvalidOfferTypeException();

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.UpdatedDate),
                values: "Date is not recent");

            var expectedOfferTypeValidatonException =
                new OfferTypeValidationException(invalidOfferTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(inputOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(
                    modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidatonException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfOfferTypeDoesNotExistAndLogItAsync()
        {
            // given
            int negativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            OfferType randomOfferType = CreateRandomOfferType(dateTime);
            OfferType nonExistOfferType = randomOfferType;
            nonExistOfferType.CreatedDate = dateTime.AddMinutes(negativeMinutes);
            OfferType nullOfferType = null;

            var notFoundOfferTypeException = new NotFoundOfferTypeException(nonExistOfferType.Id);

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(notFoundOfferTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(nonExistOfferType.Id))
                    .ReturnsAsync(nullOfferType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(nonExistOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(
                    modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(nonExistOfferType.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

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
            OfferType randomOfferType = CreateRandomModifyOfferType(randomDateTime);
            OfferType invalidOfferType = randomOfferType.DeepClone();
            OfferType storageOfferType = invalidOfferType.DeepClone();
            storageOfferType.CreatedDate = storageOfferType.CreatedDate.AddMinutes(randomMinutes);
            storageOfferType.UpdatedDate = storageOfferType.UpdatedDate.AddMinutes(randomMinutes);
            var invalidOfferTypeException = new InvalidOfferTypeException();
            Guid offerTypeId = invalidOfferType.Id;

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.CreatedDate),
                values: $"Date is not same as {nameof(OfferType.CreatedDate)}");

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(invalidOfferTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(offerTypeId)).ReturnsAsync(storageOfferType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(invalidOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(invalidOfferType.Id), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

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
            OfferType randomOfferType = CreateRandomModifyOfferType(randomDateTime);
            OfferType invalidOfferType = randomOfferType;
            OfferType storageOfferType = randomOfferType.DeepClone();
            invalidOfferType.UpdatedDate = storageOfferType.UpdatedDate;
            Guid offerTypeId = invalidOfferType.Id;
            var invalidOfferTypeException = new InvalidOfferTypeException();

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.UpdatedDate),
                values: $"Date is the same as {nameof(OfferType.UpdatedDate)}");

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(invalidOfferTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(invalidOfferType.Id)).ReturnsAsync(storageOfferType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(invalidOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(offerTypeId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
