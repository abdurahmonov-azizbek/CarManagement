// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.OfferTypes
{
    public partial class OfferTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            OfferType nullOfferType = null;
            var nullOfferTypeException = new NullOfferTypeException();

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(nullOfferTypeException);

            // when
            ValueTask<OfferType> addOfferTypeTask = this.offerTypeService.AddOfferTypeAsync(nullOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(addOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedOfferTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertOfferTypeAsync(It.IsAny<OfferType>()), Times.Never);

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
            OfferType invalidOfferType = new OfferType()
            {
                Name = invalidText
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
					values: "Date is required");



            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(invalidOfferTypeException);

            // when
            ValueTask<OfferType> addOfferTypeTask = this.offerTypeService.AddOfferTypeAsync(invalidOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(addOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertOfferTypeAsync(It.IsAny<OfferType>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            OfferType randomOfferType = CreateRandomOfferType(randomDate);
            OfferType invalidOfferType = randomOfferType;
            invalidOfferType.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidOfferTypeException = new InvalidOfferTypeException();

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.CreatedDate),
                values: $"Date is not same as {nameof(OfferType.UpdatedDate)}");

            var expectedOfferTypeValidationException = new OfferTypeValidationException(invalidOfferTypeException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<OfferType> addOfferTypeTask = this.offerTypeService.AddOfferTypeAsync(invalidOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(addOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should().BeEquivalentTo(expectedOfferTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertOfferTypeAsync(It.IsAny<OfferType>()), Times.Never);

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
            OfferType randomOfferType = CreateRandomOfferType(invalidDateTime);
            OfferType invalidOfferType = randomOfferType;
            var invalidOfferTypeException = new InvalidOfferTypeException();

            invalidOfferTypeException.AddData(
                key: nameof(OfferType.CreatedDate),
                values: "Date is not recent");

            var expectedOfferTypeValidationException =
                new OfferTypeValidationException(invalidOfferTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<OfferType> addOfferTypeTask = this.offerTypeService.AddOfferTypeAsync(invalidOfferType);

            OfferTypeValidationException actualOfferTypeValidationException =
                await Assert.ThrowsAsync<OfferTypeValidationException>(addOfferTypeTask.AsTask);

            // then
            actualOfferTypeValidationException.Should().
                BeEquivalentTo(expectedOfferTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedOfferTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertOfferTypeAsync(It.IsAny<OfferType>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}