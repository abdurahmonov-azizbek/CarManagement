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
        public async Task ShouldRemoveServiceTypeByIdAsync()
        {
            // given
            Guid randomServiceTypeId = Guid.NewGuid();
            Guid inputServiceTypeId = randomServiceTypeId;
            ServiceType randomServiceType = CreateRandomServiceType();
            ServiceType storageServiceType = randomServiceType;
            ServiceType expectedInputServiceType = storageServiceType;
            ServiceType deletedServiceType = expectedInputServiceType;
            ServiceType expectedServiceType = deletedServiceType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(inputServiceTypeId))
                    .ReturnsAsync(storageServiceType);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteServiceTypeAsync(expectedInputServiceType))
                    .ReturnsAsync(deletedServiceType);

            // when
            ServiceType actualServiceType = await this.serviceTypeService
                .RemoveServiceTypeByIdAsync(inputServiceTypeId);

            // then
            actualServiceType.Should().BeEquivalentTo(expectedServiceType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(inputServiceTypeId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteServiceTypeAsync(expectedInputServiceType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
