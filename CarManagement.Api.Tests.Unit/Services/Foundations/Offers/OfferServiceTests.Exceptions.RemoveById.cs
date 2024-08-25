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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Offers
{
    public partial class OfferServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someOfferId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedOfferException =
                new LockedOfferException(databaseUpdateConcurrencyException);

            var expectedOfferDependencyValidationException =
                new OfferDependencyValidationException(lockedOfferException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Offer> removeOfferByIdTask =
               this.offerService.RemoveOfferByIdAsync(someOfferId);

            OfferDependencyValidationException actualOfferDependencyValidationException =
                await Assert.ThrowsAsync<OfferDependencyValidationException>(
                    removeOfferByIdTask.AsTask);

            // then
            actualOfferDependencyValidationException.Should().BeEquivalentTo(
               expectedOfferDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteOfferAsync(It.IsAny<Offer>()), Times.Never);

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

            var failedOfferStorageException =
                new FailedOfferStorageException(sqlException);

            var expectedOfferDependencyException =
                new OfferDependencyException(failedOfferStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<Offer> removeOfferTask =
                this.offerService.RemoveOfferByIdAsync(someId);

            OfferDependencyException actualOfferDependencyException =
                await Assert.ThrowsAsync<OfferDependencyException>(
                    removeOfferTask.AsTask);

            // then
            actualOfferDependencyException.Should().BeEquivalentTo(expectedOfferDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someOfferId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedOfferServiceException =
                new FailedOfferServiceException(serviceException);

            var expectedOfferServiceException =
                new OfferServiceException(failedOfferServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferByIdAsync(someOfferId))
                    .Throws(serviceException);

            // when
            ValueTask<Offer> removeOfferByIdTask =
                this.offerService.RemoveOfferByIdAsync(someOfferId);

            OfferServiceException actualOfferServiceException =
                await Assert.ThrowsAsync<OfferServiceException>(
                    removeOfferByIdTask.AsTask);

            // then
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