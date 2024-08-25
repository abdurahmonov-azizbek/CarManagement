// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Offers;
using CarManagement.Api.Models.Offers.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            Offer someOffer = CreateRandomOffer();
            Guid offerId = someOffer.Id;
            SqlException sqlException = CreateSqlException();

            FailedOfferStorageException failedOfferStorageException =
                new FailedOfferStorageException(sqlException);

            OfferDependencyException expectedOfferDependencyException =
                new OfferDependencyException(failedOfferStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Offer> addOfferTask = this.offerService
                .AddOfferAsync(someOffer);

            OfferDependencyException actualOfferDependencyException =
                await Assert.ThrowsAsync<OfferDependencyException>(addOfferTask.AsTask);

            // then
            actualOfferDependencyException.Should().BeEquivalentTo(expectedOfferDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedOfferDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            Offer someOffer = CreateRandomOffer();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsOfferException =
                new AlreadyExistsOfferException(duplicateKeyException);

            var expectedOfferDependencyValidationException =
                new OfferDependencyValidationException(alreadyExistsOfferException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<Offer> addOfferTask = this.offerService.AddOfferAsync(someOffer);

            OfferDependencyValidationException actualOfferDependencyValidationException =
                await Assert.ThrowsAsync<OfferDependencyValidationException>(
                    addOfferTask.AsTask);

            // then
            actualOfferDependencyValidationException.Should().BeEquivalentTo(
                expectedOfferDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            Offer someOffer = CreateRandomOffer();
            var dbUpdateException = new DbUpdateException();

            var failedOfferStorageException = new FailedOfferStorageException(dbUpdateException);

            var expectedOfferDependencyException =
                new OfferDependencyException(failedOfferStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Offer> addOfferTask = this.offerService.AddOfferAsync(someOffer);

            OfferDependencyException actualOfferDependencyException =
                 await Assert.ThrowsAsync<OfferDependencyException>(addOfferTask.AsTask);

            // then
            actualOfferDependencyException.Should()
                .BeEquivalentTo(expectedOfferDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedOfferDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateOfferAsync(It.IsAny<Offer>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            Offer someOffer = CreateRandomOffer();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedOfferStorageException =
                new FailedOfferStorageException(dbUpdateException);

            var expectedOfferDependencyException =
                new OfferDependencyException(failedOfferStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<Offer> addOfferTask =
                this.offerService.AddOfferAsync(someOffer);

            OfferDependencyException actualOfferDependencyException =
                await Assert.ThrowsAsync<OfferDependencyException>(addOfferTask.AsTask);

            // then
            actualOfferDependencyException.Should().BeEquivalentTo(expectedOfferDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            Offer someOffer = CreateRandomOffer();
            var serviceException = new Exception();
            var failedOfferException = new FailedOfferServiceException(serviceException);

            var expectedOfferServiceExceptions =
                new OfferServiceException(failedOfferException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<Offer> addOfferTask = this.offerService.AddOfferAsync(someOffer);

            OfferServiceException actualOfferServiceException =
                await Assert.ThrowsAsync<OfferServiceException>(addOfferTask.AsTask);

            //then
            actualOfferServiceException.Should().BeEquivalentTo(
                expectedOfferServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}