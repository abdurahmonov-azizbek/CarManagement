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
        public async Task ShouldRetrieveCarModelByIdAsync()
        {
            //given
            Guid randomCarModelId = Guid.NewGuid();
            Guid inputCarModelId = randomCarModelId;
            CarModel randomCarModel = CreateRandomCarModel();
            CarModel storageCarModel = randomCarModel;
            CarModel excpectedCarModel = randomCarModel.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(inputCarModelId)).ReturnsAsync(storageCarModel);

            //when
            CarModel actuallCarModel = await this.carModelService.RetrieveCarModelByIdAsync(inputCarModelId);

            //then
            actuallCarModel.Should().BeEquivalentTo(excpectedCarModel);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(inputCarModelId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}