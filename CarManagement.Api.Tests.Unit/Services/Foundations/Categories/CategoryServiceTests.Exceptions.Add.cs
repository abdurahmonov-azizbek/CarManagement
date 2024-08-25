// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;
using CarManagement.Api.Models.Categories.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            Category someCategory = CreateRandomCategory();
            Guid categoryId = someCategory.Id;
            SqlException sqlException = CreateSqlException();

            FailedCategoryStorageException failedCategoryStorageException =
                new FailedCategoryStorageException(sqlException);

            CategoryDependencyException expectedCategoryDependencyException =
                new CategoryDependencyException(failedCategoryStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Category> addCategoryTask = this.categoryService
                .AddCategoryAsync(someCategory);

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(addCategoryTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(expectedCategoryDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedCategoryDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            Category someCategory = CreateRandomCategory();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsCategoryException =
                new AlreadyExistsCategoryException(duplicateKeyException);

            var expectedCategoryDependencyValidationException =
                new CategoryDependencyValidationException(alreadyExistsCategoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<Category> addCategoryTask = this.categoryService.AddCategoryAsync(someCategory);

            CategoryDependencyValidationException actualCategoryDependencyValidationException =
                await Assert.ThrowsAsync<CategoryDependencyValidationException>(
                    addCategoryTask.AsTask);

            // then
            actualCategoryDependencyValidationException.Should().BeEquivalentTo(
                expectedCategoryDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            Category someCategory = CreateRandomCategory();
            var dbUpdateException = new DbUpdateException();

            var failedCategoryStorageException = new FailedCategoryStorageException(dbUpdateException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(failedCategoryStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Category> addCategoryTask = this.categoryService.AddCategoryAsync(someCategory);

            CategoryDependencyException actualCategoryDependencyException =
                 await Assert.ThrowsAsync<CategoryDependencyException>(addCategoryTask.AsTask);

            // then
            actualCategoryDependencyException.Should()
                .BeEquivalentTo(expectedCategoryDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedCategoryDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCategoryAsync(It.IsAny<Category>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            Category someCategory = CreateRandomCategory();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedCategoryStorageException =
                new FailedCategoryStorageException(dbUpdateException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(failedCategoryStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<Category> addCategoryTask =
                this.categoryService.AddCategoryAsync(someCategory);

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(addCategoryTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(expectedCategoryDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            Category someCategory = CreateRandomCategory();
            var serviceException = new Exception();
            var failedCategoryException = new FailedCategoryServiceException(serviceException);

            var expectedCategoryServiceExceptions =
                new CategoryServiceException(failedCategoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<Category> addCategoryTask = this.categoryService.AddCategoryAsync(someCategory);

            CategoryServiceException actualCategoryServiceException =
                await Assert.ThrowsAsync<CategoryServiceException>(addCategoryTask.AsTask);

            //then
            actualCategoryServiceException.Should().BeEquivalentTo(
                expectedCategoryServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}