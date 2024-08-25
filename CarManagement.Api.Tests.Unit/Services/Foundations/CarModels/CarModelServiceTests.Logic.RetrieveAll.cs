// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
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
        public void ShouldRetrieveAllCarModels()
        {
            //given
            IQueryable<CarModel> randomCarModels = CreateRandomCarModels();
            IQueryable<CarModel> storageCarModels = randomCarModels;
            IQueryable<CarModel> expectedCarModels = storageCarModels.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCarModels()).Returns(storageCarModels);

            //when
            IQueryable<CarModel> actualCarModels =
                this.carModelService.RetrieveAllCarModels();

            //then
            actualCarModels.Should().BeEquivalentTo(expectedCarModels);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCarModels(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
