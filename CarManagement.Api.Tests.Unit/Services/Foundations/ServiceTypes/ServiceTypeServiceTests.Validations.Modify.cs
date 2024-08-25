// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;
using CarManagement.Api.Models.ServiceTypes.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.ServiceTypes
{
    public partial class ServiceTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfServiceTypeIsNullAndLogItAsync()
        {
            // given
            ServiceType nullServiceType = null;
            var nullServiceTypeException = new NullServiceTypeException();

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(nullServiceTypeException);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask = this.serviceTypeService.ModifyServiceTypeAsync(nullServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfServiceTypeIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            ServiceType invalidServiceType = new ServiceType
            {
                Name = invalidString
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
                values: new[]
                    {
                        "Date is required",
                        "Date is not recent",
                        $"Date is the same as {nameof(ServiceType.CreatedDate)}"
                    }
                );

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(invalidServiceTypeException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask = this.serviceTypeService.ModifyServiceTypeAsync(invalidServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            ServiceType randomServiceType = CreateRandomServiceType(randomDateTime);
            ServiceType invalidServiceType = randomServiceType;
            var invalidServiceTypeException = new InvalidServiceTypeException();

            invalidServiceTypeException.AddData(
                key: nameof(ServiceType.UpdatedDate),
                values: $"Date is the same as {nameof(ServiceType.CreatedDate)}");

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(invalidServiceTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(invalidServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                 await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(invalidServiceType.Id), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

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
            ServiceType randomServiceType = CreateRandomServiceType(dateTime);
            ServiceType inputServiceType = randomServiceType;
            inputServiceType.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidServiceTypeException = new InvalidServiceTypeException();

            invalidServiceTypeException.AddData(
                key: nameof(ServiceType.UpdatedDate),
                values: "Date is not recent");

            var expectedServiceTypeValidatonException =
                new ServiceTypeValidationException(invalidServiceTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(inputServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidatonException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfServiceTypeDoesNotExistAndLogItAsync()
        {
            // given
            int negativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            ServiceType randomServiceType = CreateRandomServiceType(dateTime);
            ServiceType nonExistServiceType = randomServiceType;
            nonExistServiceType.CreatedDate = dateTime.AddMinutes(negativeMinutes);
            ServiceType nullServiceType = null;

            var notFoundServiceTypeException = new NotFoundServiceTypeException(nonExistServiceType.Id);

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(notFoundServiceTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(nonExistServiceType.Id))
                    .ReturnsAsync(nullServiceType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(nonExistServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(nonExistServiceType.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

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
            ServiceType randomServiceType = CreateRandomModifyServiceType(randomDateTime);
            ServiceType invalidServiceType = randomServiceType.DeepClone();
            ServiceType storageServiceType = invalidServiceType.DeepClone();
            storageServiceType.CreatedDate = storageServiceType.CreatedDate.AddMinutes(randomMinutes);
            storageServiceType.UpdatedDate = storageServiceType.UpdatedDate.AddMinutes(randomMinutes);
            var invalidServiceTypeException = new InvalidServiceTypeException();
            Guid serviceTypeId = invalidServiceType.Id;

            invalidServiceTypeException.AddData(
                key: nameof(ServiceType.CreatedDate),
                values: $"Date is not same as {nameof(ServiceType.CreatedDate)}");

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(invalidServiceTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(serviceTypeId)).ReturnsAsync(storageServiceType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(invalidServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(invalidServiceType.Id), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

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
            ServiceType randomServiceType = CreateRandomModifyServiceType(randomDateTime);
            ServiceType invalidServiceType = randomServiceType;
            ServiceType storageServiceType = randomServiceType.DeepClone();
            invalidServiceType.UpdatedDate = storageServiceType.UpdatedDate;
            Guid serviceTypeId = invalidServiceType.Id;
            var invalidServiceTypeException = new InvalidServiceTypeException();

            invalidServiceTypeException.AddData(
                key: nameof(ServiceType.UpdatedDate),
                values: $"Date is the same as {nameof(ServiceType.UpdatedDate)}");

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(invalidServiceTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(invalidServiceType.Id)).ReturnsAsync(storageServiceType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<ServiceType> modifyServiceTypeTask =
                this.serviceTypeService.ModifyServiceTypeAsync(invalidServiceType);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(modifyServiceTypeTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(serviceTypeId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
