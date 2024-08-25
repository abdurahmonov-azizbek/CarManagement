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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalidServiceTypeId = Guid.Empty;
            var invalidServiceTypeException = new InvalidServiceTypeException();

            invalidServiceTypeException.AddData(
                key: nameof(ServiceType.Id),
                values: "Id is required");

            var excpectedServiceTypeValidationException = new
                ServiceTypeValidationException(invalidServiceTypeException);

            //when
            ValueTask<ServiceType> retrieveServiceTypeByIdTask =
                this.serviceTypeService.RetrieveServiceTypeByIdAsync(invalidServiceTypeId);

            ServiceTypeValidationException actuallServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    retrieveServiceTypeByIdTask.AsTask);

            //then
            actuallServiceTypeValidationException.Should()
                .BeEquivalentTo(excpectedServiceTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfServiceTypeIsNotFoundAndLogItAsync()
        {
            Guid someServiceTypeId = Guid.NewGuid();
            ServiceType noServiceType = null;

            var notFoundServiceTypeException =
                new NotFoundServiceTypeException(someServiceTypeId);

            var excpectedServiceTypeValidationException =
                new ServiceTypeValidationException(notFoundServiceTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noServiceType);

            //when 
            ValueTask<ServiceType> retrieveServiceTypeByIdTask =
                this.serviceTypeService.RetrieveServiceTypeByIdAsync(someServiceTypeId);

            ServiceTypeValidationException actualServiceTypeValidationException =
                await Assert.ThrowsAsync<ServiceTypeValidationException>(
                    retrieveServiceTypeByIdTask.AsTask);

            //then
            actualServiceTypeValidationException.Should()
                .BeEquivalentTo(excpectedServiceTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedServiceTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
