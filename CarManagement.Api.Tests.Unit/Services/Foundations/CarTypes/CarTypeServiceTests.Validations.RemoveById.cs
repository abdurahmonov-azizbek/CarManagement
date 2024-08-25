// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;
using CarManagement.Api.Models.CarTypes.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.CarTypes
{
    public partial class CarTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidCarTypeId = Guid.Empty;

            var invalidCarTypeException = new InvalidCarTypeException();

            invalidCarTypeException.AddData(
                key: nameof(CarType.Id),
                values: "Id is required");

            var expectedCarTypeValidationException =
                new CarTypeValidationException(invalidCarTypeException);

            // when
            ValueTask<CarType> removeCarTypeByIdTask =
                this.carTypeService.RemoveCarTypeByIdAsync(invalidCarTypeId);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(
                    removeCarTypeByIdTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCarTypeAsync(It.IsAny<CarType>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfCarTypeIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomCarTypeId = Guid.NewGuid();
            Guid inputCarTypeId = randomCarTypeId;
            CarType noCarType = null;

            var notFoundCarTypeException =
                new NotFoundCarTypeException(inputCarTypeId);

            var expectedCarTypeValidationException =
                new CarTypeValidationException(notFoundCarTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCarTypeByIdAsync(inputCarTypeId)).ReturnsAsync(noCarType);

            // when
            ValueTask<CarType> removeCarTypeByIdTask =
                this.carTypeService.RemoveCarTypeByIdAsync(inputCarTypeId);

            CarTypeValidationException actualCarTypeValidationException =
                await Assert.ThrowsAsync<CarTypeValidationException>(
                    removeCarTypeByIdTask.AsTask);

            // then
            actualCarTypeValidationException.Should()
                .BeEquivalentTo(expectedCarTypeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCarTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCarTypeValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
