// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;
using CarManagement.Api.Models.CarTypes.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.CarTypes
{
    public partial class CarTypeService
    {
        private delegate ValueTask<CarType> ReturningCarTypeFunction();
        private delegate IQueryable<CarType> ReturningCarTypesFunction();

        private async ValueTask<CarType> TryCatch(ReturningCarTypeFunction returningCarTypeFunction)
        {
            try
            {
                return await returningCarTypeFunction();
            }
            catch (NullCarTypeException nullCarTypeException)
            {
                throw CreateAndLogValidationException(nullCarTypeException);
            }
            catch (InvalidCarTypeException invalidCarTypeException)
            {
                throw CreateAndLogValidationException(invalidCarTypeException);
            }
            catch (NotFoundCarTypeException notFoundCarTypeException)
            {
                throw CreateAndLogValidationException(notFoundCarTypeException);
            }
            catch (SqlException sqlException)
            {
                var failedCarTypeStorageException = new FailedCarTypeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCarTypeStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsCarTypeException = new AlreadyExistsCarTypeException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsCarTypeException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedCarTypeException = new LockedCarTypeException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedCarTypeException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedCarTypeStorageException = new FailedCarTypeStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedCarTypeStorageException);
            }
            catch (Exception exception)
            {
                var failedCarTypeServiceException = new FailedCarTypeServiceException(exception);

                throw CreateAndLogServiceException(failedCarTypeServiceException);
            }
        }

        private IQueryable<CarType> TryCatch(ReturningCarTypesFunction returningCarTypesFunction)
        {
            try
            {
                return returningCarTypesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedCarTypeStorageException = new FailedCarTypeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCarTypeStorageException);
            }
            catch (Exception serviceException)
            {
                var failedCarTypeServiceException = new FailedCarTypeServiceException(serviceException);

                throw CreateAndLogServiceException(failedCarTypeServiceException);
            }
        }

        private CarTypeValidationException CreateAndLogValidationException(Xeption exception)
        {
            var carTypeValidationException = new CarTypeValidationException(exception);
            this.loggingBroker.LogError(carTypeValidationException);

            return carTypeValidationException;
        }

        private CarTypeDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var CarTypeDependencyException = new CarTypeDependencyException(exception);
            this.loggingBroker.LogCritical(CarTypeDependencyException);

            return CarTypeDependencyException;
        }

        private CarTypeDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var carTypeDependencyException = new CarTypeDependencyException(exception);
            this.loggingBroker.LogError(carTypeDependencyException);

            return carTypeDependencyException;
        }


        private CarTypeDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var carTypeDependencyValidationException = new CarTypeDependencyValidationException(exception);
            this.loggingBroker.LogError(carTypeDependencyValidationException);

            return carTypeDependencyValidationException;
        }

        private CarTypeServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var carTypeServiceException = new CarTypeServiceException(innerException);
            this.loggingBroker.LogError(carTypeServiceException);

            return carTypeServiceException;
        }
    }
}