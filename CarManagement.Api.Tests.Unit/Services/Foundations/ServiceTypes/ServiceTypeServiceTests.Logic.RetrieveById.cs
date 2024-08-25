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
        public async Task ShouldRetrieveServiceTypeByIdAsync()
        {
            //given
            Guid randomServiceTypeId = Guid.NewGuid();
            Guid inputServiceTypeId = randomServiceTypeId;
            ServiceType randomServiceType = CreateRandomServiceType();
            ServiceType storageServiceType = randomServiceType;
            ServiceType excpectedServiceType = randomServiceType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(inputServiceTypeId)).ReturnsAsync(storageServiceType);

            //when
            ServiceType actuallServiceType = await this.serviceTypeService.RetrieveServiceTypeByIdAsync(inputServiceTypeId);

            //then
            actuallServiceType.Should().BeEquivalentTo(excpectedServiceType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(inputServiceTypeId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}