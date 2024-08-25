// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedOfferStorageException(sqlException);

            var expectedOfferDependencyException =
                new OfferDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOffers()).Throws(sqlException);

            //when
            Action retrieveAllOffersAction = () =>
                this.offerService.RetrieveAllOffers();

            OfferDependencyException actualOfferDependencyException =
                Assert.Throws<OfferDependencyException>(retrieveAllOffersAction);

            //then
            actualOfferDependencyException.Should().BeEquivalentTo(
                expectedOfferDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOffers(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedOfferDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedOfferServiceException =
                new FailedOfferServiceException(serviceException);

            var expectedOfferServiceException =
                new OfferServiceException(failedOfferServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOffers()).Throws(serviceException);

            // when
            Action retrieveAllOffersAction = () =>
                this.offerService.RetrieveAllOffers();

            OfferServiceException actualOfferServiceException =
                Assert.Throws<OfferServiceException>(retrieveAllOffersAction);

            // then
            actualOfferServiceException.Should().BeEquivalentTo(expectedOfferServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOffers(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}