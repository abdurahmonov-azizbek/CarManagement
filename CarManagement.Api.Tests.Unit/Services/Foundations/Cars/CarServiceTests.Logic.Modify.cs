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
        public async Task ShouldModifyCarAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Car randomCar = CreateRandomModifyCar(randomDate);
            Car inputCar = randomCar;
            Car storageCar = inputCar.DeepClone();
            storageCar.UpdatedDate = randomCar.CreatedDate;
            Car updatedCar = inputCar;
            Car expectedCar = updatedCar.DeepClone();
            Guid carId = inputCar.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarByIdAsync(carId))
                    .ReturnsAsync(storageCar);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateCarAsync(inputCar))
                    .ReturnsAsync(updatedCar);

            // when
            Car actualCar =
               await this.carService.ModifyCarAsync(inputCar);

            // then
            actualCar.Should().BeEquivalentTo(expectedCar);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarByIdAsync(carId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarAsync(inputCar), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
