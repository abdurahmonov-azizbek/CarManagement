// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.OfferTypes
{
    public partial class OfferTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            OfferType randomOfferType = CreateRandomOfferType();
            OfferType someOfferType = randomOfferType;
            Guid offerTypeId = someOfferType.Id;
            SqlException sqlException = CreateSqlException();

            var failedOfferTypeStorageException =
                new FailedOfferTypeStorageException(sqlException);

            var expectedOfferTypeDependencyException =
                new OfferTypeDependencyException(failedOfferTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(someOfferType);

            OfferTypeDependencyException actualOfferTypeDependencyException =
                await Assert.ThrowsAsync<OfferTypeDependencyException>(
                    modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeDependencyException.Should().BeEquivalentTo(
                expectedOfferTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(offerTypeId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateOfferTypeAsync(someOfferType), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedOfferTypeDependencyException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDatetimeOffset();
            OfferType randomOfferType = CreateRandomOfferType(randomDateTime);
            OfferType someOfferType = randomOfferType;
            someOfferType.CreatedDate = someOfferType.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageOfferTypeException =
                new FailedOfferTypeStorageException(databaseUpdateException);

            var expectedOfferTypeDependencyException =
                new OfferTypeDependencyException(failedStorageOfferTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(someOfferType);

            OfferTypeDependencyException actualOfferTypeDependencyException =
                await Assert.ThrowsAsync<OfferTypeDependencyException>(
                    modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeDependencyException.Should().BeEquivalentTo(expectedOfferTypeDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeDependencyException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            OfferType randomOfferType = CreateRandomOfferType(randomDateTime);
            OfferType someOfferType = randomOfferType;
            someOfferType.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedOfferTypeException =
                new LockedOfferTypeException(databaseUpdateConcurrencyException);

            var expectedOfferTypeDependencyValidationException =
                new OfferTypeDependencyValidationException(lockedOfferTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(someOfferType);

            OfferTypeDependencyValidationException actualOfferTypeDependencyValidationException =
                await Assert.ThrowsAsync<OfferTypeDependencyValidationException>(modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeDependencyValidationException.Should()
                .BeEquivalentTo(expectedOfferTypeDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            var randomDateTime = GetRandomDateTime();
            OfferType randomOfferType = CreateRandomOfferType(randomDateTime);
            OfferType someOfferType = randomOfferType;
            someOfferType.CreatedDate = someOfferType.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedOfferTypeServiceException =
                new FailedOfferTypeServiceException(serviceException);

            var expectedOfferTypeServiceException =
                new OfferTypeServiceException(failedOfferTypeServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<OfferType> modifyOfferTypeTask =
                this.offerTypeService.ModifyOfferTypeAsync(someOfferType);

            OfferTypeServiceException actualOfferTypeServiceException =
                await Assert.ThrowsAsync<OfferTypeServiceException>(
                    modifyOfferTypeTask.AsTask);

            // then
            actualOfferTypeServiceException.Should().BeEquivalentTo(expectedOfferTypeServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
