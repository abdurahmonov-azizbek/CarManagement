// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;
using CarManagement.Api.Models.Services.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.Services
{
    public partial class ServiceService
    {
        private delegate ValueTask<Service> ReturningServiceFunction();
        private delegate IQueryable<Service> ReturningServicesFunction();

        private async ValueTask<Service> TryCatch(ReturningServiceFunction returningServiceFunction)
        {
            try
            {
                return await returningServiceFunction();
            }
            catch (NullServiceException nullServiceException)
            {
                throw CreateAndLogValidationException(nullServiceException);
            }
            catch (InvalidServiceException invalidServiceException)
            {
                throw CreateAndLogValidationException(invalidServiceException);
            }
            catch (NotFoundServiceException notFoundServiceException)
            {
                throw CreateAndLogValidationException(notFoundServiceException);
            }
            catch (SqlException sqlException)
            {
                var failedServiceStorageException = new FailedServiceStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedServiceStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsServiceException = new AlreadyExistsServiceException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsServiceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedServiceException = new LockedServiceException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedServiceException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedServiceStorageException = new FailedServiceStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedServiceStorageException);
            }
            catch (Exception exception)
            {
                var failedServiceServiceException = new FailedServiceServiceException(exception);

                throw CreateAndLogServiceException(failedServiceServiceException);
            }
        }

        private IQueryable<Service> TryCatch(ReturningServicesFunction returningServicesFunction)
        {
            try
            {
                return returningServicesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedServiceStorageException = new FailedServiceStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedServiceStorageException);
            }
            catch (Exception serviceException)
            {
                var failedServiceServiceException = new FailedServiceServiceException(serviceException);

                throw CreateAndLogServiceException(failedServiceServiceException);
            }
        }

        private ServiceValidationException CreateAndLogValidationException(Xeption exception)
        {
            var serviceValidationException = new ServiceValidationException(exception);
            this.loggingBroker.LogError(serviceValidationException);

            return serviceValidationException;
        }

        private ServiceDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var ServiceDependencyException = new ServiceDependencyException(exception);
            this.loggingBroker.LogCritical(ServiceDependencyException);

            return ServiceDependencyException;
        }

        private ServiceDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var serviceDependencyException = new ServiceDependencyException(exception);
            this.loggingBroker.LogError(serviceDependencyException);

            return serviceDependencyException;
        }


        private ServiceDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var serviceDependencyValidationException = new ServiceDependencyValidationException(exception);
            this.loggingBroker.LogError(serviceDependencyValidationException);

            return serviceDependencyValidationException;
        }

        private ServiceServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var serviceServiceException = new ServiceServiceException(innerException);
            this.loggingBroker.LogError(serviceServiceException);

            return serviceServiceException;
        }
    }
}