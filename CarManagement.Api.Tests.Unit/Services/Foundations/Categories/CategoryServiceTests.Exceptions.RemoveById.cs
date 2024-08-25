// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;
using CarManagement.Api.Models.Categories.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someCategoryId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCategoryException =
                new LockedCategoryException(databaseUpdateConcurrencyException);

            var expectedCategoryDependencyValidationException =
                new CategoryDependencyValidationException(lockedCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Category> removeCategoryByIdTask =
               this.categoryService.RemoveCategoryByIdAsync(someCategoryId);

            CategoryDependencyValidationException actualCategoryDependencyValidationException =
                await Assert.ThrowsAsync<CategoryDependencyValidationException>(
                    removeCategoryByIdTask.AsTask);

            // then
            actualCategoryDependencyValidationException.Should().BeEquivalentTo(
               expectedCategoryDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCategoryAsync(It.IsAny<Category>()), Times.Never);

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

            var failedCategoryStorageException =
                new FailedCategoryStorageException(sqlException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(failedCategoryStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<Category> removeCategoryTask =
                this.categoryService.RemoveCategoryByIdAsync(someId);

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(
                    removeCategoryTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(expectedCategoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCategoryDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someCategoryId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCategoryServiceException =
                new FailedCategoryServiceException(serviceException);

            var expectedCategoryServiceException =
                new CategoryServiceException(failedCategoryServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(someCategoryId))
                    .Throws(serviceException);

            // when
            ValueTask<Category> removeCategoryByIdTask =
                this.categoryService.RemoveCategoryByIdAsync(someCategoryId);

            CategoryServiceException actualCategoryServiceException =
                await Assert.ThrowsAsync<CategoryServiceException>(
                    removeCategoryByIdTask.AsTask);

            // then
            actualCategoryServiceException.Should().BeEquivalentTo(expectedCategoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}