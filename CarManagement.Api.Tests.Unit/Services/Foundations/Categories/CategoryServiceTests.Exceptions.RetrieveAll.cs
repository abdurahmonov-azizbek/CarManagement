// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using CarManagement.Api.Models.Categories.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoryServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedCategoryStorageException(sqlException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCategories()).Throws(sqlException);

            //when
            Action retrieveAllCategoriesAction = () =>
                this.categoryService.RetrieveAllCategories();

            CategoryDependencyException actualCategoryDependencyException =
                Assert.Throws<CategoryDependencyException>(retrieveAllCategoriesAction);

            //then
            actualCategoryDependencyException.Should().BeEquivalentTo(
                expectedCategoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCategories(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCategoryDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedCategoryServiceException =
                new FailedCategoryServiceException(serviceException);

            var expectedCategoryServiceException =
                new CategoryServiceException(failedCategoryServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCategories()).Throws(serviceException);

            // when
            Action retrieveAllCategoriesAction = () =>
                this.categoryService.RetrieveAllCategories();

            CategoryServiceException actualCategoryServiceException =
                Assert.Throws<CategoryServiceException>(retrieveAllCategoriesAction);

            // then
            actualCategoryServiceException.Should().BeEquivalentTo(expectedCategoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCategories(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}