// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.OfferTypes
{
    public partial class OfferTypeService
    {
        private delegate ValueTask<OfferType> ReturningOfferTypeFunction();
        private delegate IQueryable<OfferType> ReturningOfferTypesFunction();

        private async ValueTask<OfferType> TryCatch(ReturningOfferTypeFunction returningOfferTypeFunction)
        {
            try
            {
                return await returningOfferTypeFunction();
            }
            catch (NullOfferTypeException nullOfferTypeException)
            {
                throw CreateAndLogValidationException(nullOfferTypeException);
            }
            catch (InvalidOfferTypeException invalidOfferTypeException)
            {
                throw CreateAndLogValidationException(invalidOfferTypeException);
            }
            catch (NotFoundOfferTypeException notFoundOfferTypeException)
            {
                throw CreateAndLogValidationException(notFoundOfferTypeException);
            }
            catch (SqlException sqlException)
            {
                var failedOfferTypeStorageException = new FailedOfferTypeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedOfferTypeStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsOfferTypeException = new AlreadyExistsOfferTypeException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsOfferTypeException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedOfferTypeException = new LockedOfferTypeException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedOfferTypeException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedOfferTypeStorageException = new FailedOfferTypeStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedOfferTypeStorageException);
            }
            catch (Exception exception)
            {
                var failedOfferTypeServiceException = new FailedOfferTypeServiceException(exception);

                throw CreateAndLogServiceException(failedOfferTypeServiceException);
            }
        }

        private IQueryable<OfferType> TryCatch(ReturningOfferTypesFunction returningOfferTypesFunction)
        {
            try
            {
                return returningOfferTypesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedOfferTypeStorageException = new FailedOfferTypeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedOfferTypeStorageException);
            }
            catch (Exception serviceException)
            {
                var failedOfferTypeServiceException = new FailedOfferTypeServiceException(serviceException);

                throw CreateAndLogServiceException(failedOfferTypeServiceException);
            }
        }

        private OfferTypeValidationException CreateAndLogValidationException(Xeption exception)
        {
            var offerTypeValidationException = new OfferTypeValidationException(exception);
            this.loggingBroker.LogError(offerTypeValidationException);

            return offerTypeValidationException;
        }

        private OfferTypeDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var OfferTypeDependencyException = new OfferTypeDependencyException(exception);
            this.loggingBroker.LogCritical(OfferTypeDependencyException);

            return OfferTypeDependencyException;
        }

        private OfferTypeDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var offerTypeDependencyException = new OfferTypeDependencyException(exception);
            this.loggingBroker.LogError(offerTypeDependencyException);

            return offerTypeDependencyException;
        }


        private OfferTypeDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var offerTypeDependencyValidationException = new OfferTypeDependencyValidationException(exception);
            this.loggingBroker.LogError(offerTypeDependencyValidationException);

            return offerTypeDependencyValidationException;
        }

        private OfferTypeServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var offerTypeServiceException = new OfferTypeServiceException(innerException);
            this.loggingBroker.LogError(offerTypeServiceException);

            return offerTypeServiceException;
        }
    }
}