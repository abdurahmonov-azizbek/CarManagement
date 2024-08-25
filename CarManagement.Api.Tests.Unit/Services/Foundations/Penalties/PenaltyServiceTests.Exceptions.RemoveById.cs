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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid somePenaltyId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedPenaltyException =
                new LockedPenaltyException(databaseUpdateConcurrencyException);

            var expectedPenaltyDependencyValidationException =
                new PenaltyDependencyValidationException(lockedPenaltyException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPenaltyByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Penalty> removePenaltyByIdTask =
               this.penaltyService.RemovePenaltyByIdAsync(somePenaltyId);

            PenaltyDependencyValidationException actualPenaltyDependencyValidationException =
                await Assert.ThrowsAsync<PenaltyDependencyValidationException>(
                    removePenaltyByIdTask.AsTask);

            // then
            actualPenaltyDependencyValidationException.Should().BeEquivalentTo(
               expectedPenaltyDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPenaltyByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePenaltyAsync(It.IsAny<Penalty>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedPenaltyStorageException =
                new FailedPenaltyStorageException(sqlException);

            var expectedPenaltyDependencyException =
                new PenaltyDependencyException(failedPenaltyStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPenaltyByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<Penalty> removePenaltyTask =
                this.penaltyService.RemovePenaltyByIdAsync(someId);

            PenaltyDependencyException actualPenaltyDependencyException =
                await Assert.ThrowsAsync<PenaltyDependencyException>(
                    removePenaltyTask.AsTask);

            // then
            actualPenaltyDependencyException.Should().BeEquivalentTo(expectedPenaltyDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPenaltyByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPenaltyDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid somePenaltyId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedPenaltyServiceException =
                new FailedPenaltyServiceException(serviceException);

            var expectedPenaltyServiceException =
                new PenaltyServiceException(failedPenaltyServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPenaltyByIdAsync(somePenaltyId))
                    .Throws(serviceException);

            // when
            ValueTask<Penalty> removePenaltyByIdTask =
                this.penaltyService.RemovePenaltyByIdAsync(somePenaltyId);

            PenaltyServiceException actualPenaltyServiceException =
                await Assert.ThrowsAsync<PenaltyServiceException>(
                    removePenaltyByIdTask.AsTask);

            // then
            actualPenaltyServiceException.Should().BeEquivalentTo(expectedPenaltyServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPenaltyByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}