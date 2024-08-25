// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Brokers.DateTimes;
using CarManagement.Api.Brokers.Loggings;
using CarManagement.Api.Brokers.Storages;
using CarManagement.Api.Models.Penalties;

namespace CarManagement.Api.Services.Foundations.Penalties
{
    public partial class PenaltyService : IPenaltyService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public PenaltyService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Penalty> AddPenaltyAsync(Penalty penalty) =>
        TryCatch(async () =>
        {
            ValidatePenaltyOnAdd(penalty);

            return await this.storageBroker.InsertPenaltyAsync(penalty);
        });

        public IQueryable<Penalty> RetrieveAllPenalties() =>
            TryCatch(() => this.storageBroker.SelectAllPenalties());

        public ValueTask<Penalty> RetrievePenaltyByIdAsync(Guid penaltyId) =>
           TryCatch(async () =>
           {
               ValidatePenaltyId(penaltyId);

               Penalty maybePenalty =
                   await storageBroker.SelectPenaltyByIdAsync(penaltyId);

               ValidateStoragePenalty(maybePenalty, penaltyId);

               return maybePenalty;
           });

        public ValueTask<Penalty> ModifyPenaltyAsync(Penalty penalty) =>
            TryCatch(async () =>
            {
                ValidatePenaltyOnModify(penalty);

                Penalty maybePenalty =
                    await this.storageBroker.SelectPenaltyByIdAsync(penalty.Id);

                ValidateAgainstStoragePenaltyOnModify(inputPenalty: penalty, storagePenalty: maybePenalty);

                return await this.storageBroker.UpdatePenaltyAsync(penalty);
            });

        public ValueTask<Penalty> RemovePenaltyByIdAsync(Guid penaltyId) =>
           TryCatch(async () =>
           {
               ValidatePenaltyId(penaltyId);

               Penalty maybePenalty =
                   await this.storageBroker.SelectPenaltyByIdAsync(penaltyId);

               ValidateStoragePenalty(maybePenalty, penaltyId);

               return await this.storageBroker.DeletePenaltyAsync(maybePenalty);
           });
    }
}