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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            Category randomCategory = CreateRandomCategory();
            Category someCategory = randomCategory;
            Guid categoryId = someCategory.Id;
            SqlException sqlException = CreateSqlException();

            var failedCategoryStorageException =
                new FailedCategoryStorageException(sqlException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(failedCategoryStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(someCategory);

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(
                    modifyCategoryTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(
                expectedCategoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(categoryId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCategoryAsync(someCategory), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCategoryDependencyException))), Times.Once);

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
            Category randomCategory = CreateRandomCategory(randomDateTime);
            Category someCategory = randomCategory;
            someCategory.CreatedDate = someCategory.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageCategoryException =
                new FailedCategoryStorageException(databaseUpdateException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(failedStorageCategoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(someCategory);

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(
                    modifyCategoryTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(expectedCategoryDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryDependencyException))), Times.Once);

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
            Category randomCategory = CreateRandomCategory(randomDateTime);
            Category someCategory = randomCategory;
            someCategory.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCategoryException =
                new LockedCategoryException(databaseUpdateConcurrencyException);

            var expectedCategoryDependencyValidationException =
                new CategoryDependencyValidationException(lockedCategoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(someCategory);

            CategoryDependencyValidationException actualCategoryDependencyValidationException =
                await Assert.ThrowsAsync<CategoryDependencyValidationException>(modifyCategoryTask.AsTask);

            // then
            actualCategoryDependencyValidationException.Should()
                .BeEquivalentTo(expectedCategoryDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryDependencyValidationException))), Times.Once);

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
            Category randomCategory = CreateRandomCategory(randomDateTime);
            Category someCategory = randomCategory;
            someCategory.CreatedDate = someCategory.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedCategoryServiceException =
                new FailedCategoryServiceException(serviceException);

            var expectedCategoryServiceException =
                new CategoryServiceException(failedCategoryServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(someCategory);

            CategoryServiceException actualCategoryServiceException =
                await Assert.ThrowsAsync<CategoryServiceException>(
                    modifyCategoryTask.AsTask);

            // then
            actualCategoryServiceException.Should().BeEquivalentTo(expectedCategoryServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
