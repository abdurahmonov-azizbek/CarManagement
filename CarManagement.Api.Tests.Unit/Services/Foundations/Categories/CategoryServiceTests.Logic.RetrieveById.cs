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
        public async Task ShouldRetrieveCategoryByIdAsync()
        {
            //given
            Guid randomCategoryId = Guid.NewGuid();
            Guid inputCategoryId = randomCategoryId;
            Category randomCategory = CreateRandomCategory();
            Category storageCategory = randomCategory;
            Category excpectedCategory = randomCategory.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(inputCategoryId)).ReturnsAsync(storageCategory);

            //when
            Category actuallCategory = await this.categoryService.RetrieveCategoryByIdAsync(inputCategoryId);

            //then
            actuallCategory.Should().BeEquivalentTo(excpectedCategory);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(inputCategoryId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}