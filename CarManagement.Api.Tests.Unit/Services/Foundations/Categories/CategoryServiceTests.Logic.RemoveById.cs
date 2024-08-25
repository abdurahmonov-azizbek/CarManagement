// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace CarManagement.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoryServiceTests
    {
        [Fact]
        public async Task ShouldRemoveCategoryByIdAsync()
        {
            // given
            Guid randomCategoryId = Guid.NewGuid();
            Guid inputCategoryId = randomCategoryId;
            Category randomCategory = CreateRandomCategory();
            Category storageCategory = randomCategory;
            Category expectedInputCategory = storageCategory;
            Category deletedCategory = expectedInputCategory;
            Category expectedCategory = deletedCategory.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(inputCategoryId))
                    .ReturnsAsync(storageCategory);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteCategoryAsync(expectedInputCategory))
                    .ReturnsAsync(deletedCategory);

            // when
            Category actualCategory = await this.categoryService
                .RemoveCategoryByIdAsync(inputCategoryId);

            // then
            actualCategory.Should().BeEquivalentTo(expectedCategory);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(inputCategoryId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCategoryAsync(expectedInputCategory), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
