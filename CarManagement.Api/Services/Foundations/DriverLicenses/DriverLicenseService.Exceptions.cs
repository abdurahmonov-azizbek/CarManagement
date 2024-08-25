// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;
using CarManagement.Api.Models.DriverLicenses.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.DriverLicenses
{
    public partial class DriverLicenseService
    {
        private delegate ValueTask<DriverLicense> ReturningDriverLicenseFunction();
        private delegate IQueryable<DriverLicense> ReturningDriverLicensesFunction();

        private async ValueTask<DriverLicense> TryCatch(ReturningDriverLicenseFunction returningDriverLicenseFunction)
        {
            try
            {
                return await returningDriverLicenseFunction();
            }
            catch (NullDriverLicenseException nullDriverLicenseException)
            {
                throw CreateAndLogValidationException(nullDriverLicenseException);
            }
            catch (InvalidDriverLicenseException invalidDriverLicenseException)
            {
                throw CreateAndLogValidationException(invalidDriverLicenseException);
            }
            catch (NotFoundDriverLicenseException notFoundDriverLicenseException)
            {
                throw CreateAndLogValidationException(notFoundDriverLicenseException);
            }
            catch (SqlException sqlException)
            {
                var failedDriverLicenseStorageException = new FailedDriverLicenseStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedDriverLicenseStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsDriverLicenseException = new AlreadyExistsDriverLicenseException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsDriverLicenseException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedDriverLicenseException = new LockedDriverLicenseException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedDriverLicenseException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedDriverLicenseStorageException = new FailedDriverLicenseStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedDriverLicenseStorageException);
            }
            catch (Exception exception)
            {
                var failedDriverLicenseServiceException = new FailedDriverLicenseServiceException(exception);

                throw CreateAndLogServiceException(failedDriverLicenseServiceException);
            }
        }

        private IQueryable<DriverLicense> TryCatch(ReturningDriverLicensesFunction returningDriverLicensesFunction)
        {
            try
            {
                return returningDriverLicensesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedDriverLicenseStorageException = new FailedDriverLicenseStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedDriverLicenseStorageException);
            }
            catch (Exception serviceException)
            {
                var failedDriverLicenseServiceException = new FailedDriverLicenseServiceException(serviceException);

                throw CreateAndLogServiceException(failedDriverLicenseServiceException);
            }
        }

        private DriverLicenseValidationException CreateAndLogValidationException(Xeption exception)
        {
            var driverLicenseValidationException = new DriverLicenseValidationException(exception);
            this.loggingBroker.LogError(driverLicenseValidationException);

            return driverLicenseValidationException;
        }

        private DriverLicenseDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var DriverLicenseDependencyException = new DriverLicenseDependencyException(exception);
            this.loggingBroker.LogCritical(DriverLicenseDependencyException);

            return DriverLicenseDependencyException;
        }

        private DriverLicenseDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var driverLicenseDependencyException = new DriverLicenseDependencyException(exception);
            this.loggingBroker.LogError(driverLicenseDependencyException);

            return driverLicenseDependencyException;
        }


        private DriverLicenseDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var driverLicenseDependencyValidationException = new DriverLicenseDependencyValidationException(exception);
            this.loggingBroker.LogError(driverLicenseDependencyValidationException);

            return driverLicenseDependencyValidationException;
        }

        private DriverLicenseServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var driverLicenseServiceException = new DriverLicenseServiceException(innerException);
            this.loggingBroker.LogError(driverLicenseServiceException);

            return driverLicenseServiceException;
        }
    }
}