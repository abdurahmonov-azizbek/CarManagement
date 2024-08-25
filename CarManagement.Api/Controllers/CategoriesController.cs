// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;
using CarManagement.Api.Models.Categories.Exceptions;
using CarManagement.Api.Services.Foundations.Categories;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : RESTFulController
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService) =>
            this.categoryService = categoryService;

        [HttpPost]
        public async ValueTask<ActionResult<Category>> PostCategoryAsync(Category category)
        {
            try
            {
                Category addedCategory = await this.categoryService.AddCategoryAsync(category);

                return Created(addedCategory);
            }
            catch (CategoryValidationException categoryValidationException)
            {
                return BadRequest(categoryValidationException.InnerException);
            }
            catch (CategoryDependencyValidationException categoryDependencyValidationException)
                when (categoryDependencyValidationException.InnerException is AlreadyExistsCategoryException)
            {
                return Conflict(categoryDependencyValidationException.InnerException);
            }
            catch (CategoryDependencyException categoryDependencyException)
            {
                return InternalServerError(categoryDependencyException.InnerException);
            }
            catch (CategoryServiceException categoryServiceException)
            {
                return InternalServerError(categoryServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Category>> GetAllCategories()
        {
            try
            {
                IQueryable<Category> allCategories = this.categoryService.RetrieveAllCategories();

                return Ok(allCategories);
            }
            catch (CategoryDependencyException categoryDependencyException)
            {
                return InternalServerError(categoryDependencyException.InnerException);
            }
            catch (CategoryServiceException categoryServiceException)
            {
                return InternalServerError(categoryServiceException.InnerException);
            }
        }

        [HttpGet("{categoryId}")]
        public async ValueTask<ActionResult<Category>> GetCategoryByIdAsync(Guid categoryId)
        {
            try
            {
                return await this.categoryService.RetrieveCategoryByIdAsync(categoryId);
            }
            catch (CategoryDependencyException categoryDependencyException)
            {
                return InternalServerError(categoryDependencyException);
            }
            catch (CategoryValidationException categoryValidationException)
                when (categoryValidationException.InnerException is InvalidCategoryException)
            {
                return BadRequest(categoryValidationException.InnerException);
            }
            catch (CategoryValidationException categoryValidationException)
                 when (categoryValidationException.InnerException is NotFoundCategoryException)
            {
                return NotFound(categoryValidationException.InnerException);
            }
            catch (CategoryServiceException categoryServiceException)
            {
                return InternalServerError(categoryServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Category>> PutCategoryAsync(Category category)
        {
            try
            {
                Category modifiedCategory =
                    await this.categoryService.ModifyCategoryAsync(category);

                return Ok(modifiedCategory);
            }
            catch (CategoryValidationException categoryValidationException)
                when (categoryValidationException.InnerException is NotFoundCategoryException)
            {
                return NotFound(categoryValidationException.InnerException);
            }
            catch (CategoryValidationException categoryValidationException)
            {
                return BadRequest(categoryValidationException.InnerException);
            }
            catch (CategoryDependencyValidationException categoryDependencyValidationException)
            {
                return BadRequest(categoryDependencyValidationException.InnerException);
            }
            catch (CategoryDependencyException categoryDependencyException)
            {
                return InternalServerError(categoryDependencyException.InnerException);
            }
            catch (CategoryServiceException categoryServiceException)
            {
                return InternalServerError(categoryServiceException.InnerException);
            }
        }

        [HttpDelete("{categoryId}")]
        public async ValueTask<ActionResult<Category>> DeleteCategoryByIdAsync(Guid categoryId)
        {
            try
            {
                Category deletedCategory = await this.categoryService.RemoveCategoryByIdAsync(categoryId);

                return Ok(deletedCategory);
            }
            catch (CategoryValidationException categoryValidationException)
                when (categoryValidationException.InnerException is NotFoundCategoryException)
            {
                return NotFound(categoryValidationException.InnerException);
            }
            catch (CategoryValidationException categoryValidationException)
            {
                return BadRequest(categoryValidationException.InnerException);
            }
            catch (CategoryDependencyValidationException categoryDependencyValidationException)
                when (categoryDependencyValidationException.InnerException is LockedCategoryException)
            {
                return Locked(categoryDependencyValidationException.InnerException);
            }
            catch (CategoryDependencyValidationException categoryDependencyValidationException)
            {
                return BadRequest(categoryDependencyValidationException.InnerException);
            }
            catch (CategoryDependencyException categoryDependencyException)
            {
                return InternalServerError(categoryDependencyException.InnerException);
            }
            catch (CategoryServiceException categoryServiceException)
            {
                return InternalServerError(categoryServiceException.InnerException);
            }
        }
    }
}