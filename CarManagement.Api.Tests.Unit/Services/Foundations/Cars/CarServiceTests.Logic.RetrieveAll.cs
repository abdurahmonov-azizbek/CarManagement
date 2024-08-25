// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
using CarManagement.Api.Models.Cars;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Cars
{
    public partial class CarServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllCars()
        {
            //given
            IQueryable<Car> randomCars = CreateRandomCars();
            IQueryable<Car> storageCars = randomCars;
            IQueryable<Car> expectedCars = storageCars.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCars()).Returns(storageCars);

            //when
            IQueryable<Car> actualCars =
                this.carService.RetrieveAllCars();

            //then
            actualCars.Should().BeEquivalentTo(expectedCars);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCars(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
