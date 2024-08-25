// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Cars;
using CarManagement.Api.Models.Cars.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.Cars
{
    public partial class CarService
    {
        private delegate ValueTask<Car> ReturningCarFunction();
        private delegate IQueryable<Car> ReturningCarsFunction();

        private async ValueTask<Car> TryCatch(ReturningCarFunction returningCarFunction)
        {
            try
            {
                return await returningCarFunction();
            }
            catch (NullCarException nullCarException)
            {
                throw CreateAndLogValidationException(nullCarException);
            }
            catch (InvalidCarException invalidCarException)
            {
                throw CreateAndLogValidationException(invalidCarException);
            }
            catch (NotFoundCarException notFoundCarException)
            {
                throw CreateAndLogValidationException(notFoundCarException);
            }
            catch (SqlException sqlException)
            {
                var failedCarStorageException = new FailedCarStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCarStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsCarException = new AlreadyExistsCarException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsCarException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedCarException = new LockedCarException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedCarException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedCarStorageException = new FailedCarStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedCarStorageException);
            }
            catch (Exception exception)
            {
                var failedCarServiceException = new FailedCarServiceException(exception);

                throw CreateAndLogServiceException(failedCarServiceException);
            }
        }

        private IQueryable<Car> TryCatch(ReturningCarsFunction returningCarsFunction)
        {
            try
            {
                return returningCarsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedCarStorageException = new FailedCarStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCarStorageException);
            }
            catch (Exception serviceException)
            {
                var failedCarServiceException = new FailedCarServiceException(serviceException);

                throw CreateAndLogServiceException(failedCarServiceException);
            }
        }

        private CarValidationException CreateAndLogValidationException(Xeption exception)
        {
            var carValidationException = new CarValidationException(exception);
            this.loggingBroker.LogError(carValidationException);

            return carValidationException;
        }

        private CarDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var CarDependencyException = new CarDependencyException(exception);
            this.loggingBroker.LogCritical(CarDependencyException);

            return CarDependencyException;
        }

        private CarDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var carDependencyException = new CarDependencyException(exception);
            this.loggingBroker.LogError(carDependencyException);

            return carDependencyException;
        }


        private CarDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var carDependencyValidationException = new CarDependencyValidationException(exception);
            this.loggingBroker.LogError(carDependencyValidationException);

            return carDependencyValidationException;
        }

        private CarServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var carServiceException = new CarServiceException(innerException);
            this.loggingBroker.LogError(carServiceException);

            return carServiceException;
        }
    }
}