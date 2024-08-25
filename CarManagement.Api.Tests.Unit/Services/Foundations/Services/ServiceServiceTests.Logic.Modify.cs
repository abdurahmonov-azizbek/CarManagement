// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Services
{
    public partial class ServiceServiceTests
    {
        [Fact]
        public async Task ShouldModifyServiceAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Service randomService = CreateRandomModifyService(randomDate);
            Service inputService = randomService;
            Service storageService = inputService.DeepClone();
            storageService.UpdatedDate = randomService.CreatedDate;
            Service updatedService = inputService;
            Service expectedService = updatedService.DeepClone();
            Guid serviceId = inputService.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(serviceId))
                    .ReturnsAsync(storageService);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateServiceAsync(inputService))
                    .ReturnsAsync(updatedService);

            // when
            Service actualService =
               await this.serviceService.ModifyServiceAsync(inputService);

            // then
            actualService.Should().BeEquivalentTo(expectedService);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(serviceId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateServiceAsync(inputService), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
