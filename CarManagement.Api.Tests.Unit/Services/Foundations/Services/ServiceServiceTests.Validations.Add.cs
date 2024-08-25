// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;
using CarManagement.Api.Models.Services.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Services
{
    public partial class ServiceServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            Service nullService = null;
            var nullServiceException = new NullServiceException();

            var expectedServiceValidationException =
                new ServiceValidationException(nullServiceException);

            // when
            ValueTask<Service> addServiceTask = this.serviceService.AddServiceAsync(nullService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(addServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedServiceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceAsync(It.IsAny<Service>()), Times.Never);

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
            Service invalidService = new Service()
            {
                Name = invalidText
            };

            var invalidServiceException = new InvalidServiceException();

				invalidServiceException.AddData(
					key: nameof(Service.Id),
					values: "Id is required");

				invalidServiceException.AddData(
					key: nameof(Service.TypeId),
					values: "Id is required");

				invalidServiceException.AddData(
					key: nameof(Service.Name),
					values: "Text is required");

				invalidServiceException.AddData(
					key: nameof(Service.SertificateNumber),
					values: "Text is required");

				invalidServiceException.AddData(
					key: nameof(Service.OwnerFIO),
					values: "Text is required");

				invalidServiceException.AddData(
					key: nameof(Service.Address),
					values: "Text is required");

				invalidServiceException.AddData(
					key: nameof(Service.PhoneNumber),
					values: "Text is required");

				invalidServiceException.AddData(
					key: nameof(Service.CreatedDate),
					values: "Date is required");

				invalidServiceException.AddData(
					key: nameof(Service.UpdatedDate),
					values: "Date is required");



            var expectedServiceValidationException =
                new ServiceValidationException(invalidServiceException);

            // when
            ValueTask<Service> addServiceTask = this.serviceService.AddServiceAsync(invalidService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(addServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceAsync(It.IsAny<Service>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Service randomService = CreateRandomService(randomDate);
            Service invalidService = randomService;
            invalidService.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidServiceException = new InvalidServiceException();

            invalidServiceException.AddData(
                key: nameof(Service.CreatedDate),
                values: $"Date is not same as {nameof(Service.UpdatedDate)}");

            var expectedServiceValidationException = new ServiceValidationException(invalidServiceException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<Service> addServiceTask = this.serviceService.AddServiceAsync(invalidService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(addServiceTask.AsTask);

            // then
            actualServiceValidationException.Should().BeEquivalentTo(expectedServiceValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceAsync(It.IsAny<Service>()), Times.Never);

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
            Service randomService = CreateRandomService(invalidDateTime);
            Service invalidService = randomService;
            var invalidServiceException = new InvalidServiceException();

            invalidServiceException.AddData(
                key: nameof(Service.CreatedDate),
                values: "Date is not recent");

            var expectedServiceValidationException =
                new ServiceValidationException(invalidServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<Service> addServiceTask = this.serviceService.AddServiceAsync(invalidService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(addServiceTask.AsTask);

            // then
            actualServiceValidationException.Should().
                BeEquivalentTo(expectedServiceValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceAsync(It.IsAny<Service>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}