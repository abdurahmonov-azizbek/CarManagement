// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;
using CarManagement.Api.Models.Services.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Services
{
    public partial class ServiceServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfServiceIsNullAndLogItAsync()
        {
            // given
            Service nullService = null;
            var nullServiceException = new NullServiceException();

            var expectedServiceValidationException =
                new ServiceValidationException(nullServiceException);

            // when
            ValueTask<Service> modifyServiceTask = this.serviceService.ModifyServiceAsync(nullService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(
                    modifyServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfServiceIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            Service invalidService = new Service
            {
                Name = invalidString
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
                values: new[]
                    {
                        "Date is required",
                        "Date is not recent",
                        $"Date is the same as {nameof(Service.CreatedDate)}"
                    }
                );

            var expectedServiceValidationException =
                new ServiceValidationException(invalidServiceException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<Service> modifyServiceTask = this.serviceService.ModifyServiceAsync(invalidService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(
                    modifyServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Service randomService = CreateRandomService(randomDateTime);
            Service invalidService = randomService;
            var invalidServiceException = new InvalidServiceException();

            invalidServiceException.AddData(
                key: nameof(Service.UpdatedDate),
                values: $"Date is the same as {nameof(Service.CreatedDate)}");

            var expectedServiceValidationException =
                new ServiceValidationException(invalidServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(invalidService);

            ServiceValidationException actualServiceValidationException =
                 await Assert.ThrowsAsync<ServiceValidationException>(
                    modifyServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(invalidService.Id), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

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
            Service randomService = CreateRandomService(dateTime);
            Service inputService = randomService;
            inputService.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidServiceException = new InvalidServiceException();

            invalidServiceException.AddData(
                key: nameof(Service.UpdatedDate),
                values: "Date is not recent");

            var expectedServiceValidatonException =
                new ServiceValidationException(invalidServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(inputService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(
                    modifyServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidatonException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfServiceDoesNotExistAndLogItAsync()
        {
            // given
            int negativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            Service randomService = CreateRandomService(dateTime);
            Service nonExistService = randomService;
            nonExistService.CreatedDate = dateTime.AddMinutes(negativeMinutes);
            Service nullService = null;

            var notFoundServiceException = new NotFoundServiceException(nonExistService.Id);

            var expectedServiceValidationException =
                new ServiceValidationException(notFoundServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(nonExistService.Id))
                    .ReturnsAsync(nullService);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(nonExistService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(
                    modifyServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(nonExistService.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

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
            Service randomService = CreateRandomModifyService(randomDateTime);
            Service invalidService = randomService.DeepClone();
            Service storageService = invalidService.DeepClone();
            storageService.CreatedDate = storageService.CreatedDate.AddMinutes(randomMinutes);
            storageService.UpdatedDate = storageService.UpdatedDate.AddMinutes(randomMinutes);
            var invalidServiceException = new InvalidServiceException();
            Guid serviceId = invalidService.Id;

            invalidServiceException.AddData(
                key: nameof(Service.CreatedDate),
                values: $"Date is not same as {nameof(Service.CreatedDate)}");

            var expectedServiceValidationException =
                new ServiceValidationException(invalidServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(serviceId)).ReturnsAsync(storageService);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(invalidService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(modifyServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(invalidService.Id), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

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
            Service randomService = CreateRandomModifyService(randomDateTime);
            Service invalidService = randomService;
            Service storageService = randomService.DeepClone();
            invalidService.UpdatedDate = storageService.UpdatedDate;
            Guid serviceId = invalidService.Id;
            var invalidServiceException = new InvalidServiceException();

            invalidServiceException.AddData(
                key: nameof(Service.UpdatedDate),
                values: $"Date is the same as {nameof(Service.UpdatedDate)}");

            var expectedServiceValidationException =
                new ServiceValidationException(invalidServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(invalidService.Id)).ReturnsAsync(storageService);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Service> modifyServiceTask =
                this.serviceService.ModifyServiceAsync(invalidService);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(modifyServiceTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(serviceId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
