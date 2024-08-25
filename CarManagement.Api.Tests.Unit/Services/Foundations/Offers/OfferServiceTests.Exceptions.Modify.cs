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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            Offer randomOffer = CreateRandomOffer();
            Offer someOffer = randomOffer;
            Guid offerId = someOffer.Id;
            SqlException sqlException = CreateSqlException();

            var failedOfferStorageException =
                new FailedOfferStorageException(sqlException);

            var expectedOfferDependencyException =
                new OfferDependencyException(failedOfferStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<Offer> modifyOfferTask =
                this.offerService.ModifyOfferAsync(someOffer);

            OfferDependencyException actualOfferDependencyException =
                await Assert.ThrowsAsync<OfferDependencyException>(
                    modifyOfferTask.AsTask);

            // then
            actualOfferDependencyException.Should().BeEquivalentTo(
                expectedOfferDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferByIdAsync(offerId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateOfferAsync(someOffer), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedOfferDependencyException))), Times.Once);

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
            Offer randomOffer = CreateRandomOffer(randomDateTime);
            Offer someOffer = randomOffer;
            someOffer.CreatedDate = someOffer.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageOfferException =
                new FailedOfferStorageException(databaseUpdateException);

            var expectedOfferDependencyException =
                new OfferDependencyException(failedStorageOfferException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Offer> modifyOfferTask =
                this.offerService.ModifyOfferAsync(someOffer);

            OfferDependencyException actualOfferDependencyException =
                await Assert.ThrowsAsync<OfferDependencyException>(
                    modifyOfferTask.AsTask);

            // then
            actualOfferDependencyException.Should().BeEquivalentTo(expectedOfferDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferDependencyException))), Times.Once);

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
            Offer randomOffer = CreateRandomOffer(randomDateTime);
            Offer someOffer = randomOffer;
            someOffer.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedOfferException =
                new LockedOfferException(databaseUpdateConcurrencyException);

            var expectedOfferDependencyValidationException =
                new OfferDependencyValidationException(lockedOfferException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Offer> modifyOfferTask =
                this.offerService.ModifyOfferAsync(someOffer);

            OfferDependencyValidationException actualOfferDependencyValidationException =
                await Assert.ThrowsAsync<OfferDependencyValidationException>(modifyOfferTask.AsTask);

            // then
            actualOfferDependencyValidationException.Should()
                .BeEquivalentTo(expectedOfferDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferDependencyValidationException))), Times.Once);

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
            Offer randomOffer = CreateRandomOffer(randomDateTime);
            Offer someOffer = randomOffer;
            someOffer.CreatedDate = someOffer.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedOfferServiceException =
                new FailedOfferServiceException(serviceException);

            var expectedOfferServiceException =
                new OfferServiceException(failedOfferServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Offer> modifyOfferTask =
                this.offerService.ModifyOfferAsync(someOffer);

            OfferServiceException actualOfferServiceException =
                await Assert.ThrowsAsync<OfferServiceException>(
                    modifyOfferTask.AsTask);

            // then
            actualOfferServiceException.Should().BeEquivalentTo(expectedOfferServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
