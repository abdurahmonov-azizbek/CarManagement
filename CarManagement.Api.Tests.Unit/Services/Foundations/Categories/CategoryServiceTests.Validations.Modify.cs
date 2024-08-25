// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;
using CarManagement.Api.Models.Categories.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCategoryIsNullAndLogItAsync()
        {
            // given
            Category nullCategory = null;
            var nullCategoryException = new NullCategoryException();

            var expectedCategoryValidationException =
                new CategoryValidationException(nullCategoryException);

            // when
            ValueTask<Category> modifyCategoryTask = this.categoryService.ModifyCategoryAsync(nullCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfCategoryIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            Category invalidCategory = new Category
            {
                Name = invalidString
            };

            var invalidCategoryException = new InvalidCategoryException();

				invalidCategoryException.AddData(
					key: nameof(Category.Id),
					values: "Id is required");

				invalidCategoryException.AddData(
					key: nameof(Category.Name),
					values: "Text is required");



            invalidCategoryException.AddData(
                key: nameof(Category.CreatedDate),
                values: "Date is required");

            invalidCategoryException.AddData(
                key: nameof(Category.UpdatedDate),
                values: new[]
                    {
                        "Date is required",
                        "Date is not recent",
                        $"Date is the same as {nameof(Category.CreatedDate)}"
                    }
                );

            var expectedCategoryValidationException =
                new CategoryValidationException(invalidCategoryException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<Category> modifyCategoryTask = this.categoryService.ModifyCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Category randomCategory = CreateRandomCategory(randomDateTime);
            Category invalidCategory = randomCategory;
            var invalidCategoryException = new InvalidCategoryException();

            invalidCategoryException.AddData(
                key: nameof(Category.UpdatedDate),
                values: $"Date is the same as {nameof(Category.CreatedDate)}");

            var expectedCategoryValidationException =
                new CategoryValidationException(invalidCategoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                 await Assert.ThrowsAsync<CategoryValidationException>(
                    modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(invalidCategory.Id), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Category randomCategory = CreateRandomCategory(dateTime);
            Category inputCategory = randomCategory;
            inputCategory.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidCategoryException = new InvalidCategoryException();

            invalidCategoryException.AddData(
                key: nameof(Category.UpdatedDate),
                values: "Date is not recent");

            var expectedCategoryValidatonException =
                new CategoryValidationException(invalidCategoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(inputCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidatonException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCategoryDoesNotExistAndLogItAsync()
        {
            // given
            int negativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            Category randomCategory = CreateRandomCategory(dateTime);
            Category nonExistCategory = randomCategory;
            nonExistCategory.CreatedDate = dateTime.AddMinutes(negativeMinutes);
            Category nullCategory = null;

            var notFoundCategoryException = new NotFoundCategoryException(nonExistCategory.Id);

            var expectedCategoryValidationException =
                new CategoryValidationException(notFoundCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(nonExistCategory.Id))
                    .ReturnsAsync(nullCategory);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(nonExistCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(nonExistCategory.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Category randomCategory = CreateRandomModifyCategory(randomDateTime);
            Category invalidCategory = randomCategory.DeepClone();
            Category storageCategory = invalidCategory.DeepClone();
            storageCategory.CreatedDate = storageCategory.CreatedDate.AddMinutes(randomMinutes);
            storageCategory.UpdatedDate = storageCategory.UpdatedDate.AddMinutes(randomMinutes);
            var invalidCategoryException = new InvalidCategoryException();
            Guid categoryId = invalidCategory.Id;

            invalidCategoryException.AddData(
                key: nameof(Category.CreatedDate),
                values: $"Date is not same as {nameof(Category.CreatedDate)}");

            var expectedCategoryValidationException =
                new CategoryValidationException(invalidCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(categoryId)).ReturnsAsync(storageCategory);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(invalidCategory.Id), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Category randomCategory = CreateRandomModifyCategory(randomDateTime);
            Category invalidCategory = randomCategory;
            Category storageCategory = randomCategory.DeepClone();
            invalidCategory.UpdatedDate = storageCategory.UpdatedDate;
            Guid categoryId = invalidCategory.Id;
            var invalidCategoryException = new InvalidCategoryException();

            invalidCategoryException.AddData(
                key: nameof(Category.UpdatedDate),
                values: $"Date is the same as {nameof(Category.UpdatedDate)}");

            var expectedCategoryValidationException =
                new CategoryValidationException(invalidCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(invalidCategory.Id)).ReturnsAsync(storageCategory);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(categoryId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
