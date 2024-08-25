// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Category> InsertCategoryAsync(Category category);
        IQueryable<Category> SelectAllCategories();
        ValueTask<Category> SelectCategoryByIdAsync(Guid categoryId);
        ValueTask<Category> DeleteCategoryAsync(Category category);
        ValueTask<Category> UpdateCategoryAsync(Category category);
    }
}