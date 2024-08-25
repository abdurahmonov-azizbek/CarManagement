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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidServiceId = Guid.Empty;

            var invalidServiceException = new InvalidServiceException();

            invalidServiceException.AddData(
                key: nameof(Service.Id),
                values: "Id is required");

            var expectedServiceValidationException =
                new ServiceValidationException(invalidServiceException);

            // when
            ValueTask<Service> removeServiceByIdTask =
                this.serviceService.RemoveServiceByIdAsync(invalidServiceId);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(
                    removeServiceByIdTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteServiceAsync(It.IsAny<Service>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfServiceIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomServiceId = Guid.NewGuid();
            Guid inputServiceId = randomServiceId;
            Service noService = null;

            var notFoundServiceException =
                new NotFoundServiceException(inputServiceId);

            var expectedServiceValidationException =
                new ServiceValidationException(notFoundServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(inputServiceId)).ReturnsAsync(noService);

            // when
            ValueTask<Service> removeServiceByIdTask =
                this.serviceService.RemoveServiceByIdAsync(inputServiceId);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(
                    removeServiceByIdTask.AsTask);

            // then
            actualServiceValidationException.Should()
                .BeEquivalentTo(expectedServiceValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
