// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;
using CarManagement.Api.Models.ServiceTypes.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.ServiceTypes
{
    public partial class ServiceTypeService
    {
        private delegate ValueTask<ServiceType> ReturningServiceTypeFunction();
        private delegate IQueryable<ServiceType> ReturningServiceTypesFunction();

        private async ValueTask<ServiceType> TryCatch(ReturningServiceTypeFunction returningServiceTypeFunction)
        {
            try
            {
                return await returningServiceTypeFunction();
            }
            catch (NullServiceTypeException nullServiceTypeException)
            {
                throw CreateAndLogValidationException(nullServiceTypeException);
            }
            catch (InvalidServiceTypeException invalidServiceTypeException)
            {
                throw CreateAndLogValidationException(invalidServiceTypeException);
            }
            catch (NotFoundServiceTypeException notFoundServiceTypeException)
            {
                throw CreateAndLogValidationException(notFoundServiceTypeException);
            }
            catch (SqlException sqlException)
            {
                var failedServiceTypeStorageException = new FailedServiceTypeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedServiceTypeStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsServiceTypeException = new AlreadyExistsServiceTypeException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsServiceTypeException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedServiceTypeException = new LockedServiceTypeException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedServiceTypeException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedServiceTypeStorageException = new FailedServiceTypeStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedServiceTypeStorageException);
            }
            catch (Exception exception)
            {
                var failedServiceTypeServiceException = new FailedServiceTypeServiceException(exception);

                throw CreateAndLogServiceException(failedServiceTypeServiceException);
            }
        }

        private IQueryable<ServiceType> TryCatch(ReturningServiceTypesFunction returningServiceTypesFunction)
        {
            try
            {
                return returningServiceTypesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedServiceTypeStorageException = new FailedServiceTypeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedServiceTypeStorageException);
            }
            catch (Exception serviceException)
            {
                var failedServiceTypeServiceException = new FailedServiceTypeServiceException(serviceException);

                throw CreateAndLogServiceException(failedServiceTypeServiceException);
            }
        }

        private ServiceTypeValidationException CreateAndLogValidationException(Xeption exception)
        {
            var serviceTypeValidationException = new ServiceTypeValidationException(exception);
            this.loggingBroker.LogError(serviceTypeValidationException);

            return serviceTypeValidationException;
        }

        private ServiceTypeDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var ServiceTypeDependencyException = new ServiceTypeDependencyException(exception);
            this.loggingBroker.LogCritical(ServiceTypeDependencyException);

            return ServiceTypeDependencyException;
        }

        private ServiceTypeDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var serviceTypeDependencyException = new ServiceTypeDependencyException(exception);
            this.loggingBroker.LogError(serviceTypeDependencyException);

            return serviceTypeDependencyException;
        }


        private ServiceTypeDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var serviceTypeDependencyValidationException = new ServiceTypeDependencyValidationException(exception);
            this.loggingBroker.LogError(serviceTypeDependencyValidationException);

            return serviceTypeDependencyValidationException;
        }

        private ServiceTypeServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var serviceTypeServiceException = new ServiceTypeServiceException(innerException);
            this.loggingBroker.LogError(serviceTypeServiceException);

            return serviceTypeServiceException;
        }
    }
}