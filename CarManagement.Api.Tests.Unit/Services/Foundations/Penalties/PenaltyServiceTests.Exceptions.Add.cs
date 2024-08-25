// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Penalties;
using CarManagement.Api.Models.Penalties.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            Penalty somePenalty = CreateRandomPenalty();
            Guid penaltyId = somePenalty.Id;
            SqlException sqlException = CreateSqlException();

            FailedPenaltyStorageException failedPenaltyStorageException =
                new FailedPenaltyStorageException(sqlException);

            PenaltyDependencyException expectedPenaltyDependencyException =
                new PenaltyDependencyException(failedPenaltyStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Penalty> addPenaltyTask = this.penaltyService
                .AddPenaltyAsync(somePenalty);

            PenaltyDependencyException actualPenaltyDependencyException =
                await Assert.ThrowsAsync<PenaltyDependencyException>(addPenaltyTask.AsTask);

            // then
            actualPenaltyDependencyException.Should().BeEquivalentTo(expectedPenaltyDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedPenaltyDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            Penalty somePenalty = CreateRandomPenalty();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsPenaltyException =
                new AlreadyExistsPenaltyException(duplicateKeyException);

            var expectedPenaltyDependencyValidationException =
                new PenaltyDependencyValidationException(alreadyExistsPenaltyException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<Penalty> addPenaltyTask = this.penaltyService.AddPenaltyAsync(somePenalty);

            PenaltyDependencyValidationException actualPenaltyDependencyValidationException =
                await Assert.ThrowsAsync<PenaltyDependencyValidationException>(
                    addPenaltyTask.AsTask);

            // then
            actualPenaltyDependencyValidationException.Should().BeEquivalentTo(
                expectedPenaltyDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            Penalty somePenalty = CreateRandomPenalty();
            var dbUpdateException = new DbUpdateException();

            var failedPenaltyStorageException = new FailedPenaltyStorageException(dbUpdateException);

            var expectedPenaltyDependencyException =
                new PenaltyDependencyException(failedPenaltyStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Penalty> addPenaltyTask = this.penaltyService.AddPenaltyAsync(somePenalty);

            PenaltyDependencyException actualPenaltyDependencyException =
                 await Assert.ThrowsAsync<PenaltyDependencyException>(addPenaltyTask.AsTask);

            // then
            actualPenaltyDependencyException.Should()
                .BeEquivalentTo(expectedPenaltyDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedPenaltyDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePenaltyAsync(It.IsAny<Penalty>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            Penalty somePenalty = CreateRandomPenalty();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedPenaltyStorageException =
                new FailedPenaltyStorageException(dbUpdateException);

            var expectedPenaltyDependencyException =
                new PenaltyDependencyException(failedPenaltyStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<Penalty> addPenaltyTask =
                this.penaltyService.AddPenaltyAsync(somePenalty);

            PenaltyDependencyException actualPenaltyDependencyException =
                await Assert.ThrowsAsync<PenaltyDependencyException>(addPenaltyTask.AsTask);

            // then
            actualPenaltyDependencyException.Should().BeEquivalentTo(expectedPenaltyDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            Penalty somePenalty = CreateRandomPenalty();
            var serviceException = new Exception();
            var failedPenaltyException = new FailedPenaltyServiceException(serviceException);

            var expectedPenaltyServiceExceptions =
                new PenaltyServiceException(failedPenaltyException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<Penalty> addPenaltyTask = this.penaltyService.AddPenaltyAsync(somePenalty);

            PenaltyServiceException actualPenaltyServiceException =
                await Assert.ThrowsAsync<PenaltyServiceException>(addPenaltyTask.AsTask);

            //then
            actualPenaltyServiceException.Should().BeEquivalentTo(
                expectedPenaltyServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}