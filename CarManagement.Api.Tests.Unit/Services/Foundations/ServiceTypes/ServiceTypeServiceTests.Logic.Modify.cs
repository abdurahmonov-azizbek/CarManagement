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
        public async Task ShouldModifyServiceTypeAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            ServiceType randomServiceType = CreateRandomModifyServiceType(randomDate);
            ServiceType inputServiceType = randomServiceType;
            ServiceType storageServiceType = inputServiceType.DeepClone();
            storageServiceType.UpdatedDate = randomServiceType.CreatedDate;
            ServiceType updatedServiceType = inputServiceType;
            ServiceType expectedServiceType = updatedServiceType.DeepClone();
            Guid serviceTypeId = inputServiceType.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectServiceTypeByIdAsync(serviceTypeId))
                    .ReturnsAsync(storageServiceType);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateServiceTypeAsync(inputServiceType))
                    .ReturnsAsync(updatedServiceType);

            // when
            ServiceType actualServiceType =
               await this.serviceTypeService.ModifyServiceTypeAsync(inputServiceType);

            // then
            actualServiceType.Should().BeEquivalentTo(expectedServiceType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectServiceTypeByIdAsync(serviceTypeId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateServiceTypeAsync(inputServiceType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
