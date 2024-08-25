// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            OfferType someOfferType = CreateRandomOfferType();
            Guid offerTypeId = someOfferType.Id;
            SqlException sqlException = CreateSqlException();

            FailedOfferTypeStorageException failedOfferTypeStorageException =
                new FailedOfferTypeStorageException(sqlException);

            OfferTypeDependencyException expectedOfferTypeDependencyException =
                new OfferTypeDependencyException(failedOfferTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<OfferType> addOfferTypeTask = this.offerTypeService
                .AddOfferTypeAsync(someOfferType);

            OfferTypeDependencyException actualOfferTypeDependencyException =
                await Assert.ThrowsAsync<OfferTypeDependencyException>(addOfferTypeTask.AsTask);

            // then
            actualOfferTypeDependencyException.Should().BeEquivalentTo(expectedOfferTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedOfferTypeDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            OfferType someOfferType = CreateRandomOfferType();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsOfferTypeException =
                new AlreadyExistsOfferTypeException(duplicateKeyException);

            var expectedOfferTypeDependencyValidationException =
                new OfferTypeDependencyValidationException(alreadyExistsOfferTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<OfferType> addOfferTypeTask = this.offerTypeService.AddOfferTypeAsync(someOfferType);

            OfferTypeDependencyValidationException actualOfferTypeDependencyValidationException =
                await Assert.ThrowsAsync<OfferTypeDependencyValidationException>(
                    addOfferTypeTask.AsTask);

            // then
            actualOfferTypeDependencyValidationException.Should().BeEquivalentTo(
                expectedOfferTypeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            OfferType someOfferType = CreateRandomOfferType();
            var dbUpdateException = new DbUpdateException();

            var failedOfferTypeStorageException = new FailedOfferTypeStorageException(dbUpdateException);

            var expectedOfferTypeDependencyException =
                new OfferTypeDependencyException(failedOfferTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<OfferType> addOfferTypeTask = this.offerTypeService.AddOfferTypeAsync(someOfferType);

            OfferTypeDependencyException actualOfferTypeDependencyException =
                 await Assert.ThrowsAsync<OfferTypeDependencyException>(addOfferTypeTask.AsTask);

            // then
            actualOfferTypeDependencyException.Should()
                .BeEquivalentTo(expectedOfferTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedOfferTypeDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateOfferTypeAsync(It.IsAny<OfferType>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            OfferType someOfferType = CreateRandomOfferType();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedOfferTypeStorageException =
                new FailedOfferTypeStorageException(dbUpdateException);

            var expectedOfferTypeDependencyException =
                new OfferTypeDependencyException(failedOfferTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<OfferType> addOfferTypeTask =
                this.offerTypeService.AddOfferTypeAsync(someOfferType);

            OfferTypeDependencyException actualOfferTypeDependencyException =
                await Assert.ThrowsAsync<OfferTypeDependencyException>(addOfferTypeTask.AsTask);

            // then
            actualOfferTypeDependencyException.Should().BeEquivalentTo(expectedOfferTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            OfferType someOfferType = CreateRandomOfferType();
            var serviceException = new Exception();
            var failedOfferTypeException = new FailedOfferTypeServiceException(serviceException);

            var expectedOfferTypeServiceExceptions =
                new OfferTypeServiceException(failedOfferTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<OfferType> addOfferTypeTask = this.offerTypeService.AddOfferTypeAsync(someOfferType);

            OfferTypeServiceException actualOfferTypeServiceException =
                await Assert.ThrowsAsync<OfferTypeServiceException>(addOfferTypeTask.AsTask);

            //then
            actualOfferTypeServiceException.Should().BeEquivalentTo(
                expectedOfferTypeServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}