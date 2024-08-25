// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Penalties;
using CarManagement.Api.Models.Penalties.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace CarManagement.Api.Services.Foundations.Penalties
{
    public partial class PenaltyService
    {
        private delegate ValueTask<Penalty> ReturningPenaltyFunction();
        private delegate IQueryable<Penalty> ReturningPenaltiesFunction();

        private async ValueTask<Penalty> TryCatch(ReturningPenaltyFunction returningPenaltyFunction)
        {
            try
            {
                return await returningPenaltyFunction();
            }
            catch (NullPenaltyException nullPenaltyException)
            {
                throw CreateAndLogValidationException(nullPenaltyException);
            }
            catch (InvalidPenaltyException invalidPenaltyException)
            {
                throw CreateAndLogValidationException(invalidPenaltyException);
            }
            catch (NotFoundPenaltyException notFoundPenaltyException)
            {
                throw CreateAndLogValidationException(notFoundPenaltyException);
            }
            catch (SqlException sqlException)
            {
                var failedPenaltyStorageException = new FailedPenaltyStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedPenaltyStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsPenaltyException = new AlreadyExistsPenaltyException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsPenaltyException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedPenaltyException = new LockedPenaltyException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedPenaltyException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedPenaltyStorageException = new FailedPenaltyStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedPenaltyStorageException);
            }
            catch (Exception exception)
            {
                var failedPenaltyServiceException = new FailedPenaltyServiceException(exception);

                throw CreateAndLogServiceException(failedPenaltyServiceException);
            }
        }

        private IQueryable<Penalty> TryCatch(ReturningPenaltiesFunction returningPenaltiesFunction)
        {
            try
            {
                return returningPenaltiesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedPenaltyStorageException = new FailedPenaltyStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedPenaltyStorageException);
            }
            catch (Exception serviceException)
            {
                var failedPenaltyServiceException = new FailedPenaltyServiceException(serviceException);

                throw CreateAndLogServiceException(failedPenaltyServiceException);
            }
        }

        private PenaltyValidationException CreateAndLogValidationException(Xeption exception)
        {
            var penaltyValidationException = new PenaltyValidationException(exception);
            this.loggingBroker.LogError(penaltyValidationException);

            return penaltyValidationException;
        }

        private PenaltyDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var PenaltyDependencyException = new PenaltyDependencyException(exception);
            this.loggingBroker.LogCritical(PenaltyDependencyException);

            return PenaltyDependencyException;
        }

        private PenaltyDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var penaltyDependencyException = new PenaltyDependencyException(exception);
            this.loggingBroker.LogError(penaltyDependencyException);

            return penaltyDependencyException;
        }


        private PenaltyDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var penaltyDependencyValidationException = new PenaltyDependencyValidationException(exception);
            this.loggingBroker.LogError(penaltyDependencyValidationException);

            return penaltyDependencyValidationException;
        }

        private PenaltyServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var penaltyServiceException = new PenaltyServiceException(innerException);
            this.loggingBroker.LogError(penaltyServiceException);

            return penaltyServiceException;
        }
    }
}