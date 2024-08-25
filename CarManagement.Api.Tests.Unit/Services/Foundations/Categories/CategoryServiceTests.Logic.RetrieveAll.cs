// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System.Linq;
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
        public void ShouldRetrieveAllCategories()
        {
            //given
            IQueryable<Category> randomCategories = CreateRandomCategories();
            IQueryable<Category> storageCategories = randomCategories;
            IQueryable<Category> expectedCategories = storageCategories.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCategories()).Returns(storageCategories);

            //when
            IQueryable<Category> actualCategories =
                this.categoryService.RetrieveAllCategories();

            //then
            actualCategories.Should().BeEquivalentTo(expectedCategories);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCategories(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
