// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
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
        public void ShouldRetrieveAllServices()
        {
            //given
            IQueryable<Service> randomServices = CreateRandomServices();
            IQueryable<Service> storageServices = randomServices;
            IQueryable<Service> expectedServices = storageServices.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllServices()).Returns(storageServices);

            //when
            IQueryable<Service> actualServices =
                this.serviceService.RetrieveAllServices();

            //then
            actualServices.Should().BeEquivalentTo(expectedServices);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllServices(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
