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
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            Category nullCategory = null;
            var nullCategoryException = new NullCategoryException();

            var expectedCategoryValidationException =
                new CategoryValidationException(nullCategoryException);

            // when
            ValueTask<Category> addCategoryTask = this.categoryService.AddCategoryAsync(nullCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(addCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedCategoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(It.IsAny<Category>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfJobIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            Category invalidCategory = new Category()
            {
                Name = invalidText
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
					values: "Date is required");



            var expectedCategoryValidationException =
                new CategoryValidationException(invalidCategoryException);

            // when
            ValueTask<Category> addCategoryTask = this.categoryService.AddCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(addCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(It.IsAny<Category>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Category randomCategory = CreateRandomCategory(randomDate);
            Category invalidCategory = randomCategory;
            invalidCategory.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidCategoryException = new InvalidCategoryException();

            invalidCategoryException.AddData(
                key: nameof(Category.CreatedDate),
                values: $"Date is not same as {nameof(Category.UpdatedDate)}");

            var expectedCategoryValidationException = new CategoryValidationException(invalidCategoryException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<Category> addCategoryTask = this.categoryService.AddCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(addCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should().BeEquivalentTo(expectedCategoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(It.IsAny<Category>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinutes))]
        public async Task ShouldThrowValidationExceptionIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidMinutes)
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            DateTimeOffset invalidDateTime = randomDate.AddMinutes(invalidMinutes);
            Category randomCategory = CreateRandomCategory(invalidDateTime);
            Category invalidCategory = randomCategory;
            var invalidCategoryException = new InvalidCategoryException();

            invalidCategoryException.AddData(
                key: nameof(Category.CreatedDate),
                values: "Date is not recent");

            var expectedCategoryValidationException =
                new CategoryValidationException(invalidCategoryException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<Category> addCategoryTask = this.categoryService.AddCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(addCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should().
                BeEquivalentTo(expectedCategoryValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(It.IsAny<Category>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}