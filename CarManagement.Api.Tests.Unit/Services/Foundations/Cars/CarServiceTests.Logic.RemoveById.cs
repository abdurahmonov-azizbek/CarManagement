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
        public async Task ShouldRemoveCarByIdAsync()
        {
            // given
            Guid randomCarId = Guid.NewGuid();
            Guid inputCarId = randomCarId;
            Car randomCar = CreateRandomCar();
            Car storageCar = randomCar;
            Car expectedInputCar = storageCar;
            Car deletedCar = expectedInputCar;
            Car expectedCar = deletedCar.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarByIdAsync(inputCarId))
                    .ReturnsAsync(storageCar);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteCarAsync(expectedInputCar))
                    .ReturnsAsync(deletedCar);

            // when
            Car actualCar = await this.carService
                .RemoveCarByIdAsync(inputCarId);

            // then
            actualCar.Should().BeEquivalentTo(expectedCar);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarByIdAsync(inputCarId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCarAsync(expectedInputCar), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
