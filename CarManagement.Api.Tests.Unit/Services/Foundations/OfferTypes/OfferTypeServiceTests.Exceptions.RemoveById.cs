// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;
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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someOfferTypeId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedOfferTypeException =
                new LockedOfferTypeException(databaseUpdateConcurrencyException);

            var expectedOfferTypeDependencyValidationException =
                new OfferTypeDependencyValidationException(lockedOfferTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<OfferType> removeOfferTypeByIdTask =
               this.offerTypeService.RemoveOfferTypeByIdAsync(someOfferTypeId);

            OfferTypeDependencyValidationException actualOfferTypeDependencyValidationException =
                await Assert.ThrowsAsync<OfferTypeDependencyValidationException>(
                    removeOfferTypeByIdTask.AsTask);

            // then
            actualOfferTypeDependencyValidationException.Should().BeEquivalentTo(
               expectedOfferTypeDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteOfferTypeAsync(It.IsAny<OfferType>()), Times.Never);

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

            var failedOfferTypeStorageException =
                new FailedOfferTypeStorageException(sqlException);

            var expectedOfferTypeDependencyException =
                new OfferTypeDependencyException(failedOfferTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<OfferType> removeOfferTypeTask =
                this.offerTypeService.RemoveOfferTypeByIdAsync(someId);

            OfferTypeDependencyException actualOfferTypeDependencyException =
                await Assert.ThrowsAsync<OfferTypeDependencyException>(
                    removeOfferTypeTask.AsTask);

            // then
            actualOfferTypeDependencyException.Should().BeEquivalentTo(expectedOfferTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedOfferTypeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someOfferTypeId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedOfferTypeServiceException =
                new FailedOfferTypeServiceException(serviceException);

            var expectedOfferTypeServiceException =
                new OfferTypeServiceException(failedOfferTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(someOfferTypeId))
                    .Throws(serviceException);

            // when
            ValueTask<OfferType> removeOfferTypeByIdTask =
                this.offerTypeService.RemoveOfferTypeByIdAsync(someOfferTypeId);

            OfferTypeServiceException actualOfferTypeServiceException =
                await Assert.ThrowsAsync<OfferTypeServiceException>(
                    removeOfferTypeByIdTask.AsTask);

            // then
            actualOfferTypeServiceException.Should().BeEquivalentTo(expectedOfferTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}