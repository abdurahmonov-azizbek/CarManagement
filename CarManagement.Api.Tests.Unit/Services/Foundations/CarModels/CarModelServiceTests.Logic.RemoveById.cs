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
        public async Task ShouldRemoveCarModelByIdAsync()
        {
            // given
            Guid randomCarModelId = Guid.NewGuid();
            Guid inputCarModelId = randomCarModelId;
            CarModel randomCarModel = CreateRandomCarModel();
            CarModel storageCarModel = randomCarModel;
            CarModel expectedInputCarModel = storageCarModel;
            CarModel deletedCarModel = expectedInputCarModel;
            CarModel expectedCarModel = deletedCarModel.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(inputCarModelId))
                    .ReturnsAsync(storageCarModel);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteCarModelAsync(expectedInputCarModel))
                    .ReturnsAsync(deletedCarModel);

            // when
            CarModel actualCarModel = await this.carModelService
                .RemoveCarModelByIdAsync(inputCarModelId);

            // then
            actualCarModel.Should().BeEquivalentTo(expectedCarModel);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(inputCarModelId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCarModelAsync(expectedInputCarModel), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
