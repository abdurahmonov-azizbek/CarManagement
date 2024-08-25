// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedOfferTypeStorageException(sqlException);

            var expectedOfferTypeDependencyException =
                new OfferTypeDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOfferTypes()).Throws(sqlException);

            //when
            Action retrieveAllOfferTypesAction = () =>
                this.offerTypeService.RetrieveAllOfferTypes();

            OfferTypeDependencyException actualOfferTypeDependencyException =
                Assert.Throws<OfferTypeDependencyException>(retrieveAllOfferTypesAction);

            //then
            actualOfferTypeDependencyException.Should().BeEquivalentTo(
                expectedOfferTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOfferTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedOfferTypeDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedOfferTypeServiceException =
                new FailedOfferTypeServiceException(serviceException);

            var expectedOfferTypeServiceException =
                new OfferTypeServiceException(failedOfferTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOfferTypes()).Throws(serviceException);

            // when
            Action retrieveAllOfferTypesAction = () =>
                this.offerTypeService.RetrieveAllOfferTypes();

            OfferTypeServiceException actualOfferTypeServiceException =
                Assert.Throws<OfferTypeServiceException>(retrieveAllOfferTypesAction);

            // then
            actualOfferTypeServiceException.Should().BeEquivalentTo(expectedOfferTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOfferTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOfferTypeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}