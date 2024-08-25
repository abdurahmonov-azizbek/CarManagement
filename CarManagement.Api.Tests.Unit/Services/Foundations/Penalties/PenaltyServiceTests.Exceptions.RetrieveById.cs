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
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Penalties
{
    public partial class PenaltyServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedPenaltyStorageException =
                new FailedPenaltyStorageException(sqlException);

            var expectedPenaltyDependencyException =
                new PenaltyDependencyException(failedPenaltyStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPenaltyByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<Penalty> retrievePenaltyByIdTask =
                this.penaltyService.RetrievePenaltyByIdAsync(someId);

            PenaltyDependencyException actualPenaltyDependencyexception =
                await Assert.ThrowsAsync<PenaltyDependencyException>(
                    retrievePenaltyByIdTask.AsTask);

            //then
            actualPenaltyDependencyexception.Should().BeEquivalentTo(
                expectedPenaltyDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedPenaltyServiceException =
                new FailedPenaltyServiceException(serviceException);

            var expectedPenaltyServiceException =
                new PenaltyServiceException(failedPenaltyServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPenaltyByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<Penalty> retrievePenaltyByIdTask =
                this.penaltyService.RetrievePenaltyByIdAsync(someId);

            PenaltyServiceException actualPenaltyServiceException =
                await Assert.ThrowsAsync<PenaltyServiceException>(retrievePenaltyByIdTask.AsTask);

            //then
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