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
        public async Task ShouldAddCarAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Car randomCar = CreateRandomCar(randomDate);
            Car inputCar = randomCar;
            Car persistedCar = inputCar;
            Car expectedCar = persistedCar.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCarAsync(inputCar)).ReturnsAsync(persistedCar);

            // when
            Car actualCar = await this.carService
                .AddCarAsync(inputCar);

            // then
            actualCar.Should().BeEquivalentTo(expectedCar);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarAsync(inputCar), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}