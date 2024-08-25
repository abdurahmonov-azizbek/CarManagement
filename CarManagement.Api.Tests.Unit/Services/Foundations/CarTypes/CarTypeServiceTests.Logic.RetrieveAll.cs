// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
using CarManagement.Api.Models.CarTypes;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarTypes
{
    public partial class CarTypeServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllCarTypes()
        {
            //given
            IQueryable<CarType> randomCarTypes = CreateRandomCarTypes();
            IQueryable<CarType> storageCarTypes = randomCarTypes;
            IQueryable<CarType> expectedCarTypes = storageCarTypes.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCarTypes()).Returns(storageCarTypes);

            //when
            IQueryable<CarType> actualCarTypes =
                this.carTypeService.RetrieveAllCarTypes();

            //then
            actualCarTypes.Should().BeEquivalentTo(expectedCarTypes);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCarTypes(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
