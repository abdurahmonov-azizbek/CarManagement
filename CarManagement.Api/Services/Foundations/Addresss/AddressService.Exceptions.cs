// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Addresss;
using CarManagement.Api.Models.Addresss.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.Addresss
{
    public partial class AddressService
    {
        private delegate ValueTask<Address> ReturningAddressFunction();
        private delegate IQueryable<Address> ReturningAddresssFunction();

        private async ValueTask<Address> TryCatch(ReturningAddressFunction returningAddressFunction)
        {
            try
            {
                return await returningAddressFunction();
            }
            catch (NullAddressException nullAddressException)
            {
                throw CreateAndLogValidationException(nullAddressException);
            }
            catch (InvalidAddressException invalidAddressException)
            {
                throw CreateAndLogValidationException(invalidAddressException);
            }
            catch (NotFoundAddressException notFoundAddressException)
            {
                throw CreateAndLogValidationException(notFoundAddressException);
            }
            catch (SqlException sqlException)
            {
                var failedAddressStorageException = new FailedAddressStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedAddressStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsAddressException = new AlreadyExistsAddressException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsAddressException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedAddressException = new LockedAddressException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedAddressException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedAddressStorageException = new FailedAddressStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedAddressStorageException);
            }
            catch (Exception exception)
            {
                var failedAddressServiceException = new FailedAddressServiceException(exception);

                throw CreateAndLogServiceException(failedAddressServiceException);
            }
        }

        private IQueryable<Address> TryCatch(ReturningAddresssFunction returningAddresssFunction)
        {
            try
            {
                return returningAddresssFunction();
            }
            catch (SqlException sqlException)
            {
                var failedAddressStorageException = new FailedAddressStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedAddressStorageException);
            }
            catch (Exception serviceException)
            {
                var failedAddressServiceException = new FailedAddressServiceException(serviceException);

                throw CreateAndLogServiceException(failedAddressServiceException);
            }
        }

        private AddressValidationException CreateAndLogValidationException(Xeption exception)
        {
            var addressValidationException = new AddressValidationException(exception);
            this.loggingBroker.LogError(addressValidationException);

            return addressValidationException;
        }

        private AddressDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var AddressDependencyException = new AddressDependencyException(exception);
            this.loggingBroker.LogCritical(AddressDependencyException);

            return AddressDependencyException;
        }

        private AddressDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var addressDependencyException = new AddressDependencyException(exception);
            this.loggingBroker.LogError(addressDependencyException);

            return addressDependencyException;
        }


        private AddressDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var addressDependencyValidationException = new AddressDependencyValidationException(exception);
            this.loggingBroker.LogError(addressDependencyValidationException);

            return addressDependencyValidationException;
        }

        private AddressServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var addressServiceException = new AddressServiceException(innerException);
            this.loggingBroker.LogError(addressServiceException);

            return addressServiceException;
        }
    }
}