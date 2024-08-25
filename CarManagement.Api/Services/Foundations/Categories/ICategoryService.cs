// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;

namespace CarManagement.Api.Services.Foundations.Categories
{
    public interface ICategoryService  
    {
        /// <exception cref="Models.Categories.Exceptions.CategoryValidationException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryDependencyValidationException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryDependencyException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryServiceException"></exception>
        ValueTask<Category> AddCategoryAsync(Category category);

        /// <exception cref="Models.Categories.Exceptions.CategoryDependencyException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryServiceException"></exception>
        IQueryable<Category> RetrieveAllCategories();

        /// <exception cref="Models.Categories.Exceptions.CategoryDependencyException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryServiceException"></exception>
        ValueTask<Category> RetrieveCategoryByIdAsync(Guid categoryId);

        /// <exception cref="Models.Categories.Exceptions.CategoryValidationException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryDependencyValidationException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryDependencyException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryServiceException"></exception>
        ValueTask<Category> ModifyCategoryAsync(Category category);

        /// <exception cref="Models.Categories.Exceptions.CategoryDependencyValidationException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryDependencyException"></exception>
        /// <exception cref="Models.Categories.Exceptions.CategoryServiceException"></exception>
        ValueTask<Category> RemoveCategoryByIdAsync(Guid categoryId);
    }
}