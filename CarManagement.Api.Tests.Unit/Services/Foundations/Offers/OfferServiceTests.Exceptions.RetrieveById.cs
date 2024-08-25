// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Offers;
using CarManagement.Api.Models.Offers.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Offers
{
    public partial class OfferServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedOfferStorageException =
                new FailedOfferStorageException(sqlException);

            var expectedOfferDependencyException =
                new OfferDependencyException(failedOfferStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<Offer> retrieveOfferByIdTask =
                this.offerService.RetrieveOfferByIdAsync(someId);

            OfferDependencyException actualOfferDependencyexception =
                await Assert.ThrowsAsync<OfferDependencyException>(
                    retrieveOfferByIdTask.AsTask);

            //then
            actualOfferDependencyexception.Should().BeEquivalentTo(
                expectedOfferDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedOfferDependencyException))), Times.Once);

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

            var failedOfferServiceException =
                new FailedOfferServiceException(serviceException);

            var expectedOfferServiceException =
                new OfferServiceException(failedOfferServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<Offer> retrieveOfferByIdTask =
                this.offerService.RetrieveOfferByIdAsync(someId);

            OfferServiceException actualOfferServiceException =
                await Assert.ThrowsAsync<OfferServiceException>(retrieveOfferByIdTask.AsTask);

            //then
            actualOfferServiceException.Should().BeEquivalentTo(expectedOfferServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}