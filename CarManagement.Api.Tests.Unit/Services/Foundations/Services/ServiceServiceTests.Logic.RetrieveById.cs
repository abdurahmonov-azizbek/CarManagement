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
        public async Task ShouldRetrieveServiceByIdAsync()
        {
            //given
            Guid randomServiceId = Guid.NewGuid();
            Guid inputServiceId = randomServiceId;
            Service randomService = CreateRandomService();
            Service storageService = randomService;
            Service excpectedService = randomService.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceByIdAsync(inputServiceId)).ReturnsAsync(storageService);

            //when
            Service actuallService = await this.serviceService.RetrieveServiceByIdAsync(inputServiceId);

            //then
            actuallService.Should().BeEquivalentTo(excpectedService);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceByIdAsync(inputServiceId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}