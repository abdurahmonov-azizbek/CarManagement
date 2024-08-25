// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarModels
{
    public partial class CarModelServiceTests
    {
        [Fact]
        public async Task ShouldAddCarModelAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            CarModel randomCarModel = CreateRandomCarModel(randomDate);
            CarModel inputCarModel = randomCarModel;
            CarModel persistedCarModel = inputCarModel;
            CarModel expectedCarModel = persistedCarModel.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCarModelAsync(inputCarModel)).ReturnsAsync(persistedCarModel);

            // when
            CarModel actualCarModel = await this.carModelService
                .AddCarModelAsync(inputCarModel);

            // then
            actualCarModel.Should().BeEquivalentTo(expectedCarModel);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCarModelAsync(inputCarModel), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}