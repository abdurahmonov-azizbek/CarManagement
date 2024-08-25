// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;
using CarManagement.Api.Models.ServiceTypes.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.ServiceTypes
{
    public partial class ServiceTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            ServiceType nullServiceType = null;
            var nullServiceTypeException = new NullServiceTypeException();

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(nullServiceTypeException);

            // when
            ValueTask<ServiceType> addServiceTypeTask = this.serviceTypeService.AddServiceTypeAsync(nullServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(addServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedServiceTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceTypeAsync(It.IsAny<ServiceType>()), Times.Never);

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
            ServiceType invalidServiceType = new ServiceType()
            {
                Name = invalidText
            };

            var invalidServiceTypeException = new InvalidServiceTypeException();

				invalidServiceTypeException.AddData(
					key: nameof(ServiceType.Id),
					values: "Id is required");

				invalidServiceTypeException.AddData(
					key: nameof(ServiceType.Name),
					values: "Text is required");

				invalidServiceTypeException.AddData(
					key: nameof(ServiceType.CreatedDate),
					values: "Date is required");

				invalidServiceTypeException.AddData(
					key: nameof(ServiceType.UpdatedDate),
					values: "Date is required");



            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(invalidServiceTypeException);

            // when
            ValueTask<ServiceType> addServiceTypeTask = this.serviceTypeService.AddServiceTypeAsync(invalidServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(addServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceTypeAsync(It.IsAny<ServiceType>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            ServiceType randomServiceType = CreateRandomServiceType(randomDate);
            ServiceType invalidServiceType = randomServiceType;
            invalidServiceType.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidServiceTypeException = new InvalidServiceTypeException();

            invalidServiceTypeException.AddData(
                key: nameof(ServiceType.CreatedDate),
                values: $"Date is not same as {nameof(ServiceType.UpdatedDate)}");

            var expectedServiceTypeValidationException = new ServiceTypeValidationException(invalidServiceTypeException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<ServiceType> addServiceTypeTask = this.serviceTypeService.AddServiceTypeAsync(invalidServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(addServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should().BeEquivalentTo(expectedServiceTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceTypeAsync(It.IsAny<ServiceType>()), Times.Never);

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
            ServiceType randomServiceType = CreateRandomServiceType(invalidDateTime);
            ServiceType invalidServiceType = randomServiceType;
            var invalidServiceTypeException = new InvalidServiceTypeException();

            invalidServiceTypeException.AddData(
                key: nameof(ServiceType.CreatedDate),
                values: "Date is not recent");

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(invalidServiceTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<ServiceType> addServiceTypeTask = this.serviceTypeService.AddServiceTypeAsync(invalidServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(addServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should().
                BeEquivalentTo(expectedServiceTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceTypeAsync(It.IsAny<ServiceType>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}