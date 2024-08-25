// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Offers;
using CarManagement.Api.Models.Offers.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.Offers
{
    public partial class OfferService
    {
        private delegate ValueTask<Offer> ReturningOfferFunction();
        private delegate IQueryable<Offer> ReturningOffersFunction();

        private async ValueTask<Offer> TryCatch(ReturningOfferFunction returningOfferFunction)
        {
            try
            {
                return await returningOfferFunction();
            }
            catch (NullOfferException nullOfferException)
            {
                throw CreateAndLogValidationException(nullOfferException);
            }
            catch (InvalidOfferException invalidOfferException)
            {
                throw CreateAndLogValidationException(invalidOfferException);
            }
            catch (NotFoundOfferException notFoundOfferException)
            {
                throw CreateAndLogValidationException(notFoundOfferException);
            }
            catch (SqlException sqlException)
            {
                var failedOfferStorageException = new FailedOfferStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedOfferStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsOfferException = new AlreadyExistsOfferException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsOfferException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedOfferException = new LockedOfferException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedOfferException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedOfferStorageException = new FailedOfferStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedOfferStorageException);
            }
            catch (Exception exception)
            {
                var failedOfferServiceException = new FailedOfferServiceException(exception);

                throw CreateAndLogServiceException(failedOfferServiceException);
            }
        }

        private IQueryable<Offer> TryCatch(ReturningOffersFunction returningOffersFunction)
        {
            try
            {
                return returningOffersFunction();
            }
            catch (SqlException sqlException)
            {
                var failedOfferStorageException = new FailedOfferStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedOfferStorageException);
            }
            catch (Exception serviceException)
            {
                var failedOfferServiceException = new FailedOfferServiceException(serviceException);

                throw CreateAndLogServiceException(failedOfferServiceException);
            }
        }

        private OfferValidationException CreateAndLogValidationException(Xeption exception)
        {
            var offerValidationException = new OfferValidationException(exception);
            this.loggingBroker.LogError(offerValidationException);

            return offerValidationException;
        }

        private OfferDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var OfferDependencyException = new OfferDependencyException(exception);
            this.loggingBroker.LogCritical(OfferDependencyException);

            return OfferDependencyException;
        }

        private OfferDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var offerDependencyException = new OfferDependencyException(exception);
            this.loggingBroker.LogError(offerDependencyException);

            return offerDependencyException;
        }


        private OfferDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var offerDependencyValidationException = new OfferDependencyValidationException(exception);
            this.loggingBroker.LogError(offerDependencyValidationException);

            return offerDependencyValidationException;
        }

        private OfferServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var offerServiceException = new OfferServiceException(innerException);
            this.loggingBroker.LogError(offerServiceException);

            return offerServiceException;
        }
    }
}