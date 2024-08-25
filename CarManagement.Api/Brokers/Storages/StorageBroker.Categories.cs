// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Category> Categories { get; set; }

        public async ValueTask<Category> InsertCategoryAsync(Category category) =>
            await InsertAsync(category);

        public IQueryable<Category> SelectAllCategories() =>
            SelectAll<Category>();

        public async ValueTask<Category> SelectCategoryByIdAsync(Guid categoryId) =>
            await SelectAsync<Category>(categoryId);

        public async ValueTask<Category> DeleteCategoryAsync(Category category) =>
            await DeleteAsync(category);

        public async ValueTask<Category> UpdateCategoryAsync(Category category) =>
            await UpdateAsync(category);
    }
}