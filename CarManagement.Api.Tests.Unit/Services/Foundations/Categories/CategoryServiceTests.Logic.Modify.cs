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
        public async Task ShouldModifyCategoryAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Category randomCategory = CreateRandomModifyCategory(randomDate);
            Category inputCategory = randomCategory;
            Category storageCategory = inputCategory.DeepClone();
            storageCategory.UpdatedDate = randomCategory.CreatedDate;
            Category updatedCategory = inputCategory;
            Category expectedCategory = updatedCategory.DeepClone();
            Guid categoryId = inputCategory.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(categoryId))
                    .ReturnsAsync(storageCategory);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateCategoryAsync(inputCategory))
                    .ReturnsAsync(updatedCategory);

            // when
            Category actualCategory =
               await this.categoryService.ModifyCategoryAsync(inputCategory);

            // then
            actualCategory.Should().BeEquivalentTo(expectedCategory);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(categoryId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCategoryAsync(inputCategory), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
