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
        public async Task ShouldRemoveServiceByIdAsync()
        {
            // given
            Guid randomServiceId = Guid.NewGuid();
            Guid inputServiceId = randomServiceId;
            Service randomService = CreateRandomService();
            Service storageService = randomService;
            Service expectedInputService = storageService;
            Service deletedService = expectedInputService;
            Service expectedService = deletedService.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(inputServiceId))
                    .ReturnsAsync(storageService);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteServiceAsync(expectedInputService))
                    .ReturnsAsync(deletedService);

            // when
            Service actualService = await this.serviceService
                .RemoveServiceByIdAsync(inputServiceId);

            // then
            actualService.Should().BeEquivalentTo(expectedService);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(inputServiceId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteServiceAsync(expectedInputService), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
