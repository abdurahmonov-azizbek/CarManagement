// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Brokers.DateTimes;
using CarManagement.Api.Brokers.Loggings;
using CarManagement.Api.Brokers.Storages;
using CarManagement.Api.Models.Categories;

namespace CarManagement.Api.Services.Foundations.Categories
{
    public partial class CategoryService : ICategoryService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public CategoryService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Category> AddCategoryAsync(Category category) =>
        TryCatch(async () =>
        {
            ValidateCategoryOnAdd(category);

            return await this.storageBroker.InsertCategoryAsync(category);
        });

        public IQueryable<Category> RetrieveAllCategories() =>
            TryCatch(() => this.storageBroker.SelectAllCategories());

        public ValueTask<Category> RetrieveCategoryByIdAsync(Guid categoryId) =>
           TryCatch(async () =>
           {
               ValidateCategoryId(categoryId);

               Category maybeCategory =
                   await storageBroker.SelectCategoryByIdAsync(categoryId);

               ValidateStorageCategory(maybeCategory, categoryId);

               return maybeCategory;
           });

        public ValueTask<Category> ModifyCategoryAsync(Category category) =>
            TryCatch(async () =>
            {
                ValidateCategoryOnModify(category);

                Category maybeCategory =
                    await this.storageBroker.SelectCategoryByIdAsync(category.Id);

                ValidateAgainstStorageCategoryOnModify(inputCategory: category, storageCategory: maybeCategory);

                return await this.storageBroker.UpdateCategoryAsync(category);
            });

        public ValueTask<Category> RemoveCategoryByIdAsync(Guid categoryId) =>
           TryCatch(async () =>
           {
               ValidateCategoryId(categoryId);

               Category maybeCategory =
                   await this.storageBroker.SelectCategoryByIdAsync(categoryId);

               ValidateStorageCategory(maybeCategory, categoryId);

               return await this.storageBroker.DeleteCategoryAsync(maybeCategory);
           });
    }
}