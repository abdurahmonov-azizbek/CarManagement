// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldAddCarTypeAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            CarType randomCarType = CreateRandomCarType(randomDate);
            CarType inputCarType = randomCarType;
            CarType persistedCarType = inputCarType;
            CarType expectedCarType = persistedCarType.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCarTypeAsync(inputCarType)).ReturnsAsync(persistedCarType);

            // when
            CarType actualCarType = await this.carTypeService
                .AddCarTypeAsync(inputCarType);

            // then
            actualCarType.Should().BeEquivalentTo(expectedCarType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarTypeAsync(inputCarType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}