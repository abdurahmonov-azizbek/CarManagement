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
        public async Task ShouldAddServiceAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Service randomService = CreateRandomService(randomDate);
            Service inputService = randomService;
            Service persistedService = inputService;
            Service expectedService = persistedService.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertServiceAsync(inputService)).ReturnsAsync(persistedService);

            // when
            Service actualService = await this.serviceService
                .AddServiceAsync(inputService);

            // then
            actualService.Should().BeEquivalentTo(expectedService);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceAsync(inputService), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}