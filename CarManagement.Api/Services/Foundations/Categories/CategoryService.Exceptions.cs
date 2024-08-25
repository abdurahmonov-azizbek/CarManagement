// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Categories;
using CarManagement.Api.Models.Categories.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.Categories
{
    public partial class CategoryService
    {
        private delegate ValueTask<Category> ReturningCategoryFunction();
        private delegate IQueryable<Category> ReturningCategoriesFunction();

        private async ValueTask<Category> TryCatch(ReturningCategoryFunction returningCategoryFunction)
        {
            try
            {
                return await returningCategoryFunction();
            }
            catch (NullCategoryException nullCategoryException)
            {
                throw CreateAndLogValidationException(nullCategoryException);
            }
            catch (InvalidCategoryException invalidCategoryException)
            {
                throw CreateAndLogValidationException(invalidCategoryException);
            }
            catch (NotFoundCategoryException notFoundCategoryException)
            {
                throw CreateAndLogValidationException(notFoundCategoryException);
            }
            catch (SqlException sqlException)
            {
                var failedCategoryStorageException = new FailedCategoryStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCategoryStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsCategoryException = new AlreadyExistsCategoryException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsCategoryException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedCategoryException = new LockedCategoryException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedCategoryException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedCategoryStorageException = new FailedCategoryStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedCategoryStorageException);
            }
            catch (Exception exception)
            {
                var failedCategoryServiceException = new FailedCategoryServiceException(exception);

                throw CreateAndLogServiceException(failedCategoryServiceException);
            }
        }

        private IQueryable<Category> TryCatch(ReturningCategoriesFunction returningCategoriesFunction)
        {
            try
            {
                return returningCategoriesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedCategoryStorageException = new FailedCategoryStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCategoryStorageException);
            }
            catch (Exception serviceException)
            {
                var failedCategoryServiceException = new FailedCategoryServiceException(serviceException);

                throw CreateAndLogServiceException(failedCategoryServiceException);
            }
        }

        private CategoryValidationException CreateAndLogValidationException(Xeption exception)
        {
            var categoryValidationException = new CategoryValidationException(exception);
            this.loggingBroker.LogError(categoryValidationException);

            return categoryValidationException;
        }

        private CategoryDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var CategoryDependencyException = new CategoryDependencyException(exception);
            this.loggingBroker.LogCritical(CategoryDependencyException);

            return CategoryDependencyException;
        }

        private CategoryDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var categoryDependencyException = new CategoryDependencyException(exception);
            this.loggingBroker.LogError(categoryDependencyException);

            return categoryDependencyException;
        }


        private CategoryDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var categoryDependencyValidationException = new CategoryDependencyValidationException(exception);
            this.loggingBroker.LogError(categoryDependencyValidationException);

            return categoryDependencyValidationException;
        }

        private CategoryServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var categoryServiceException = new CategoryServiceException(innerException);
            this.loggingBroker.LogError(categoryServiceException);

            return categoryServiceException;
        }
    }
}