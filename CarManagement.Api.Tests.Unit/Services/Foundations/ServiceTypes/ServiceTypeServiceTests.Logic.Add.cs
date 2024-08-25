// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.ServiceTypes
{
    public partial class ServiceTypeServiceTests
    {
        [Fact]
        public async Task ShouldAddServiceTypeAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            ServiceType randomServiceType = CreateRandomServiceType(randomDate);
            ServiceType inputServiceType = randomServiceType;
            ServiceType persistedServiceType = inputServiceType;
            ServiceType expectedServiceType = persistedServiceType.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertServiceTypeAsync(inputServiceType)).ReturnsAsync(persistedServiceType);

            // when
            ServiceType actualServiceType = await this.serviceTypeService
                .AddServiceTypeAsync(inputServiceType);

            // then
            actualServiceType.Should().BeEquivalentTo(expectedServiceType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertServiceTypeAsync(inputServiceType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}