// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;
using CarManagement.Api.Models.CarModels.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.CarModels
{
    public partial class CarModelService
    {
        private delegate ValueTask<CarModel> ReturningCarModelFunction();
        private delegate IQueryable<CarModel> ReturningCarModelsFunction();

        private async ValueTask<CarModel> TryCatch(ReturningCarModelFunction returningCarModelFunction)
        {
            try
            {
                return await returningCarModelFunction();
            }
            catch (NullCarModelException nullCarModelException)
            {
                throw CreateAndLogValidationException(nullCarModelException);
            }
            catch (InvalidCarModelException invalidCarModelException)
            {
                throw CreateAndLogValidationException(invalidCarModelException);
            }
            catch (NotFoundCarModelException notFoundCarModelException)
            {
                throw CreateAndLogValidationException(notFoundCarModelException);
            }
            catch (SqlException sqlException)
            {
                var failedCarModelStorageException = new FailedCarModelStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCarModelStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsCarModelException = new AlreadyExistsCarModelException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsCarModelException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedCarModelException = new LockedCarModelException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedCarModelException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedCarModelStorageException = new FailedCarModelStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedCarModelStorageException);
            }
            catch (Exception exception)
            {
                var failedCarModelServiceException = new FailedCarModelServiceException(exception);

                throw CreateAndLogServiceException(failedCarModelServiceException);
            }
        }

        private IQueryable<CarModel> TryCatch(ReturningCarModelsFunction returningCarModelsFunction)
        {
            try
            {
                return returningCarModelsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedCarModelStorageException = new FailedCarModelStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCarModelStorageException);
            }
            catch (Exception serviceException)
            {
                var failedCarModelServiceException = new FailedCarModelServiceException(serviceException);

                throw CreateAndLogServiceException(failedCarModelServiceException);
            }
        }

        private CarModelValidationException CreateAndLogValidationException(Xeption exception)
        {
            var carModelValidationException = new CarModelValidationException(exception);
            this.loggingBroker.LogError(carModelValidationException);

            return carModelValidationException;
        }

        private CarModelDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var CarModelDependencyException = new CarModelDependencyException(exception);
            this.loggingBroker.LogCritical(CarModelDependencyException);

            return CarModelDependencyException;
        }

        private CarModelDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var carModelDependencyException = new CarModelDependencyException(exception);
            this.loggingBroker.LogError(carModelDependencyException);

            return carModelDependencyException;
        }


        private CarModelDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var carModelDependencyValidationException = new CarModelDependencyValidationException(exception);
            this.loggingBroker.LogError(carModelDependencyValidationException);

            return carModelDependencyValidationException;
        }

        private CarModelServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var carModelServiceException = new CarModelServiceException(innerException);
            this.loggingBroker.LogError(carModelServiceException);

            return carModelServiceException;
        }
    }
}