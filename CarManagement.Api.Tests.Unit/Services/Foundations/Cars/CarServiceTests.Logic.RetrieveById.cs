// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldRetrieveCarByIdAsync()
        {
            //given
            Guid randomCarId = Guid.NewGuid();
            Guid inputCarId = randomCarId;
            Car randomCar = CreateRandomCar();
            Car storageCar = randomCar;
            Car excpectedCar = randomCar.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarByIdAsync(inputCarId)).ReturnsAsync(storageCar);

            //when
            Car actuallCar = await this.carService.RetrieveCarByIdAsync(inputCarId);

            //then
            actuallCar.Should().BeEquivalentTo(excpectedCar);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarByIdAsync(inputCarId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}