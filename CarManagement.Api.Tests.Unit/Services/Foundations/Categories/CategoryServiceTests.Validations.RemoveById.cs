// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;
using CarManagement.Api.Models.Categories.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidCategoryId = Guid.Empty;

            var invalidCategoryException = new InvalidCategoryException();

            invalidCategoryException.AddData(
                key: nameof(Category.Id),
                values: "Id is required");

            var expectedCategoryValidationException =
                new CategoryValidationException(invalidCategoryException);

            // when
            ValueTask<Category> removeCategoryByIdTask =
                this.categoryService.RemoveCategoryByIdAsync(invalidCategoryId);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    removeCategoryByIdTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCategoryAsync(It.IsAny<Category>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveIfCategoryIsNotFoundAndLogItAsync()
        {
            // given
            Guid randomCategoryId = Guid.NewGuid();
            Guid inputCategoryId = randomCategoryId;
            Category noCategory = null;

            var notFoundCategoryException =
                new NotFoundCategoryException(inputCategoryId);

            var expectedCategoryValidationException =
                new CategoryValidationException(notFoundCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(inputCategoryId)).ReturnsAsync(noCategory);

            // when
            ValueTask<Category> removeCategoryByIdTask =
                this.categoryService.RemoveCategoryByIdAsync(inputCategoryId);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    removeCategoryByIdTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
