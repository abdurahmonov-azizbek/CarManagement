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
        public async Task ShouldAddCategoryAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Category randomCategory = CreateRandomCategory(randomDate);
            Category inputCategory = randomCategory;
            Category persistedCategory = inputCategory;
            Category expectedCategory = persistedCategory.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCategoryAsync(inputCategory)).ReturnsAsync(persistedCategory);

            // when
            Category actualCategory = await this.categoryService
                .AddCategoryAsync(inputCategory);

            // then
            actualCategory.Should().BeEquivalentTo(expectedCategory);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(inputCategory), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}