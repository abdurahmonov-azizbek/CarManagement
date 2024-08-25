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
        public async Task ShouldRemoveCarTypeByIdAsync()
        {
            // given
            Guid randomCarTypeId = Guid.NewGuid();
            Guid inputCarTypeId = randomCarTypeId;
            CarType randomCarType = CreateRandomCarType();
            CarType storageCarType = randomCarType;
            CarType expectedInputCarType = storageCarType;
            CarType deletedCarType = expectedInputCarType;
            CarType expectedCarType = deletedCarType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(inputCarTypeId))
                    .ReturnsAsync(storageCarType);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteCarTypeAsync(expectedInputCarType))
                    .ReturnsAsync(deletedCarType);

            // when
            CarType actualCarType = await this.carTypeService
                .RemoveCarTypeByIdAsync(inputCarTypeId);

            // then
            actualCarType.Should().BeEquivalentTo(expectedCarType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(inputCarTypeId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCarTypeAsync(expectedInputCarType), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
