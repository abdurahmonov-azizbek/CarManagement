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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalidServiceId = Guid.Empty;
            var invalidServiceException = new InvalidServiceException();

            invalidServiceException.AddData(
                key: nameof(Service.Id),
                values: "Id is required");

            var excpectedServiceValidationException = new
                ServiceValidationException(invalidServiceException);

            //when
            ValueTask<Service> retrieveServiceByIdTask =
                this.serviceService.RetrieveServiceByIdAsync(invalidServiceId);

            ServiceValidationException actuallServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(
                    retrieveServiceByIdTask.AsTask);

            //then
            actuallServiceValidationException.Should()
                .BeEquivalentTo(excpectedServiceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfServiceIsNotFoundAndLogItAsync()
        {
            Guid someServiceId = Guid.NewGuid();
            Service noService = null;

            var notFoundServiceException =
                new NotFoundServiceException(someServiceId);

            var excpectedServiceValidationException =
                new ServiceValidationException(notFoundServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noService);

            //when 
            ValueTask<Service> retrieveServiceByIdTask =
                this.serviceService.RetrieveServiceByIdAsync(someServiceId);

            ServiceValidationException actualServiceValidationException =
                await Assert.ThrowsAsync<ServiceValidationException>(
                    retrieveServiceByIdTask.AsTask);

            //then
            actualServiceValidationException.Should()
                .BeEquivalentTo(excpectedServiceValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedServiceValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
