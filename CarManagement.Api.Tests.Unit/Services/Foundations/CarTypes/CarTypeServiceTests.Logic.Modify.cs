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
        public async Task ShouldModifyCarTypeAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            CarType randomCarType = CreateRandomModifyCarType(randomDate);
            CarType inputCarType = randomCarType;
            CarType storageCarType = inputCarType.DeepClone();
            storageCarType.UpdatedDate = randomCarType.CreatedDate;
            CarType updatedCarType = inputCarType;
            CarType expectedCarType = updatedCarType.DeepClone();
            Guid carTypeId = inputCarType.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(carTypeId))
                    .ReturnsAsync(storageCarType);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateCarTypeAsync(inputCarType))
                    .ReturnsAsync(updatedCarType);

            // when
            CarType actualCarType =
               await this.carTypeService.ModifyCarTypeAsync(inputCarType);

            // then
            actualCarType.Should().BeEquivalentTo(expectedCarType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(carTypeId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCarTypeAsync(inputCarType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
