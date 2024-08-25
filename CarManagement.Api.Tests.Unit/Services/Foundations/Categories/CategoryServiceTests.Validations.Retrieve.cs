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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalidCategoryId = Guid.Empty;
            var invalidCategoryException = new InvalidCategoryException();

            invalidCategoryException.AddData(
                key: nameof(Category.Id),
                values: "Id is required");

            var excpectedCategoryValidationException = new
                CategoryValidationException(invalidCategoryException);

            //when
            ValueTask<Category> retrieveCategoryByIdTask =
                this.categoryService.RetrieveCategoryByIdAsync(invalidCategoryId);

            CategoryValidationException actuallCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    retrieveCategoryByIdTask.AsTask);

            //then
            actuallCategoryValidationException.Should()
                .BeEquivalentTo(excpectedCategoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfCategoryIsNotFoundAndLogItAsync()
        {
            Guid someCategoryId = Guid.NewGuid();
            Category noCategory = null;

            var notFoundCategoryException =
                new NotFoundCategoryException(someCategoryId);

            var excpectedCategoryValidationException =
                new CategoryValidationException(notFoundCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noCategory);

            //when 
            ValueTask<Category> retrieveCategoryByIdTask =
                this.categoryService.RetrieveCategoryByIdAsync(someCategoryId);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    retrieveCategoryByIdTask.AsTask);

            //then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(excpectedCategoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
