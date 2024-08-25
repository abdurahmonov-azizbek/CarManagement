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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidServiceTypeId = Guid.Empty;

            var invalidServiceTypeException = new InvalidServiceTypeException();

            invalidServiceTypeException.AddData(
                key: nameof(ServiceType.Id),
                values: "Id is required");

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(invalidServiceTypeException);

            // when
            ValueTask<ServiceType> removeServiceTypeByIdTask =
                this.serviceTypeService.RemoveServiceTypeByIdAsync(invalidServiceTypeId);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    removeServiceTypeByIdTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteServiceTypeAsync(It.IsAny<ServiceType>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfServiceTypeIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomServiceTypeId = Guid.NewGuid();
            Guid inputServiceTypeId = randomServiceTypeId;
            ServiceType noServiceType = null;

            var notFoundServiceTypeException =
                new NotFoundServiceTypeException(inputServiceTypeId);

            var expectedServiceTypeValidationException =
                new ServiceTypeValidationException(notFoundServiceTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(inputServiceTypeId)).ReturnsAsync(noServiceType);

            // when
            ValueTask<ServiceType> removeServiceTypeByIdTask =
                this.serviceTypeService.RemoveServiceTypeByIdAsync(inputServiceTypeId);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    removeServiceTypeByIdTask.AsTask);

            // then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(expectedServiceTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
