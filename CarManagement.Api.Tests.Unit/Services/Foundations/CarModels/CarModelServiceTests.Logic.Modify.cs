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
        public async Task ShouldModifyCarModelAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            CarModel randomCarModel = CreateRandomModifyCarModel(randomDate);
            CarModel inputCarModel = randomCarModel;
            CarModel storageCarModel = inputCarModel.DeepClone();
            storageCarModel.UpdatedDate = randomCarModel.CreatedDate;
            CarModel updatedCarModel = inputCarModel;
            CarModel expectedCarModel = updatedCarModel.DeepClone();
            Guid carModelId = inputCarModel.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarModelByIdAsync(carModelId))
                    .ReturnsAsync(storageCarModel);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateCarModelAsync(inputCarModel))
                    .ReturnsAsync(updatedCarModel);

            // when
            CarModel actualCarModel =
               await this.carModelService.ModifyCarModelAsync(inputCarModel);

            // then
            actualCarModel.Should().BeEquivalentTo(expectedCarModel);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarModelByIdAsync(carModelId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarModelAsync(inputCarModel), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
