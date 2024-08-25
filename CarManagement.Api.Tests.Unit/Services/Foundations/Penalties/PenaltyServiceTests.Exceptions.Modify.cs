// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Penalties;
using CarManagement.Api.Models.Penalties.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Penalties
{
    public partial class PenaltyServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            Penalty randomPenalty = CreateRandomPenalty();
            Penalty somePenalty = randomPenalty;
            Guid penaltyId = somePenalty.Id;
            SqlException sqlException = CreateSqlException();

            var failedPenaltyStorageException =
                new FailedPenaltyStorageException(sqlException);

            var expectedPenaltyDependencyException =
                new PenaltyDependencyException(failedPenaltyStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<Penalty> modifyPenaltyTask =
                this.penaltyService.ModifyPenaltyAsync(somePenalty);

            PenaltyDependencyException actualPenaltyDependencyException =
                await Assert.ThrowsAsync<PenaltyDependencyException>(
                    modifyPenaltyTask.AsTask);

            // then
            actualPenaltyDependencyException.Should().BeEquivalentTo(
                expectedPenaltyDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPenaltyByIdAsync(penaltyId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePenaltyAsync(somePenalty), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPenaltyDependencyException))), Times.Once);

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
            Penalty randomPenalty = CreateRandomPenalty(randomDateTime);
            Penalty somePenalty = randomPenalty;
            somePenalty.CreatedDate = somePenalty.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStoragePenaltyException =
                new FailedPenaltyStorageException(databaseUpdateException);

            var expectedPenaltyDependencyException =
                new PenaltyDependencyException(failedStoragePenaltyException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Penalty> modifyPenaltyTask =
                this.penaltyService.ModifyPenaltyAsync(somePenalty);

            PenaltyDependencyException actualPenaltyDependencyException =
                await Assert.ThrowsAsync<PenaltyDependencyException>(
                    modifyPenaltyTask.AsTask);

            // then
            actualPenaltyDependencyException.Should().BeEquivalentTo(expectedPenaltyDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyDependencyException))), Times.Once);

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
            Penalty randomPenalty = CreateRandomPenalty(randomDateTime);
            Penalty somePenalty = randomPenalty;
            somePenalty.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedPenaltyException =
                new LockedPenaltyException(databaseUpdateConcurrencyException);

            var expectedPenaltyDependencyValidationException =
                new PenaltyDependencyValidationException(lockedPenaltyException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Penalty> modifyPenaltyTask =
                this.penaltyService.ModifyPenaltyAsync(somePenalty);

            PenaltyDependencyValidationException actualPenaltyDependencyValidationException =
                await Assert.ThrowsAsync<PenaltyDependencyValidationException>(modifyPenaltyTask.AsTask);

            // then
            actualPenaltyDependencyValidationException.Should()
                .BeEquivalentTo(expectedPenaltyDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyDependencyValidationException))), Times.Once);

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
            Penalty randomPenalty = CreateRandomPenalty(randomDateTime);
            Penalty somePenalty = randomPenalty;
            somePenalty.CreatedDate = somePenalty.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedPenaltyServiceException =
                new FailedPenaltyServiceException(serviceException);

            var expectedPenaltyServiceException =
                new PenaltyServiceException(failedPenaltyServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Penalty> modifyPenaltyTask =
                this.penaltyService.ModifyPenaltyAsync(somePenalty);

            PenaltyServiceException actualPenaltyServiceException =
                await Assert.ThrowsAsync<PenaltyServiceException>(
                    modifyPenaltyTask.AsTask);

            // then
            actualPenaltyServiceException.Should().BeEquivalentTo(expectedPenaltyServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
