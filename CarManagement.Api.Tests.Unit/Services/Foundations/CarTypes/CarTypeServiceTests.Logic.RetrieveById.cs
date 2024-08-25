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
        public async Task ShouldRetrieveCarTypeByIdAsync()
        {
            //given
            Guid randomCarTypeId = Guid.NewGuid();
            Guid inputCarTypeId = randomCarTypeId;
            CarType randomCarType = CreateRandomCarType();
            CarType storageCarType = randomCarType;
            CarType excpectedCarType = randomCarType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(inputCarTypeId)).ReturnsAsync(storageCarType);

            //when
            CarType actuallCarType = await this.carTypeService.RetrieveCarTypeByIdAsync(inputCarTypeId);

            //then
            actuallCarType.Should().BeEquivalentTo(excpectedCarType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(inputCarTypeId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}