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
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.OfferTypes
{
    public partial class OfferTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedOfferTypeStorageException =
                new FailedOfferTypeStorageException(sqlException);

            var expectedOfferTypeDependencyException =
                new OfferTypeDependencyException(failedOfferTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<OfferType> retrieveOfferTypeByIdTask =
                this.offerTypeService.RetrieveOfferTypeByIdAsync(someId);

            OfferTypeDependencyException actualOfferTypeDependencyexception =
                await Assert.ThrowsAsync<OfferTypeDependencyException>(
                    retrieveOfferTypeByIdTask.AsTask);

            //then
            actualOfferTypeDependencyexception.Should().BeEquivalentTo(
                expectedOfferTypeDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedOfferTypeServiceException =
                new FailedOfferTypeServiceException(serviceException);

            var expectedOfferTypeServiceException =
                new OfferTypeServiceException(failedOfferTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOfferTypeByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<OfferType> retrieveOfferTypeByIdTask =
                this.offerTypeService.RetrieveOfferTypeByIdAsync(someId);

            OfferTypeServiceException actualOfferTypeServiceException =
                await Assert.ThrowsAsync<OfferTypeServiceException>(retrieveOfferTypeByIdTask.AsTask);

            //then
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