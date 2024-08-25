// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
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
        public void ShouldRetrieveAllServiceTypes()
        {
            //given
            IQueryable<ServiceType> randomServiceTypes = CreateRandomServiceTypes();
            IQueryable<ServiceType> storageServiceTypes = randomServiceTypes;
            IQueryable<ServiceType> expectedServiceTypes = storageServiceTypes.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllServiceTypes()).Returns(storageServiceTypes);

            //when
            IQueryable<ServiceType> actualServiceTypes =
                this.serviceTypeService.RetrieveAllServiceTypes();

            //then
            actualServiceTypes.Should().BeEquivalentTo(expectedServiceTypes);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllServiceTypes(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
