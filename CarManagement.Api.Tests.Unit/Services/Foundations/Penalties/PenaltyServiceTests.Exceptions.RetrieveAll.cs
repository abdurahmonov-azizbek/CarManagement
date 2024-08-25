// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedPenaltyStorageException(sqlException);

            var expectedPenaltyDependencyException =
                new PenaltyDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPenalties()).Throws(sqlException);

            //when
            Action retrieveAllPenaltiesAction = () =>
                this.penaltyService.RetrieveAllPenalties();

            PenaltyDependencyException actualPenaltyDependencyException =
                Assert.Throws<PenaltyDependencyException>(retrieveAllPenaltiesAction);

            //then
            actualPenaltyDependencyException.Should().BeEquivalentTo(
                expectedPenaltyDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPenalties(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPenaltyDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedPenaltyServiceException =
                new FailedPenaltyServiceException(serviceException);

            var expectedPenaltyServiceException =
                new PenaltyServiceException(failedPenaltyServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPenalties()).Throws(serviceException);

            // when
            Action retrieveAllPenaltiesAction = () =>
                this.penaltyService.RetrieveAllPenalties();

            PenaltyServiceException actualPenaltyServiceException =
                Assert.Throws<PenaltyServiceException>(retrieveAllPenaltiesAction);

            // then
            actualPenaltyServiceException.Should().BeEquivalentTo(expectedPenaltyServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPenalties(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPenaltyServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}