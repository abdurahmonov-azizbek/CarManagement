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
using CarManagement.Api.Models.OfferTypes;

namespace CarManagement.Api.Services.Foundations.OfferTypes
{
    public partial class OfferTypeService : IOfferTypeService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public OfferTypeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<OfferType> AddOfferTypeAsync(OfferType offerType) =>
        TryCatch(async () =>
        {
            ValidateOfferTypeOnAdd(offerType);

            return await this.storageBroker.InsertOfferTypeAsync(offerType);
        });

        public IQueryable<OfferType> RetrieveAllOfferTypes() =>
            TryCatch(() => this.storageBroker.SelectAllOfferTypes());

        public ValueTask<OfferType> RetrieveOfferTypeByIdAsync(Guid offerTypeId) =>
           TryCatch(async () =>
           {
               ValidateOfferTypeId(offerTypeId);

               OfferType maybeOfferType =
                   await storageBroker.SelectOfferTypeByIdAsync(offerTypeId);

               ValidateStorageOfferType(maybeOfferType, offerTypeId);

               return maybeOfferType;
           });

        public ValueTask<OfferType> ModifyOfferTypeAsync(OfferType offerType) =>
            TryCatch(async () =>
            {
                ValidateOfferTypeOnModify(offerType);

                OfferType maybeOfferType =
                    await this.storageBroker.SelectOfferTypeByIdAsync(offerType.Id);

                ValidateAgainstStorageOfferTypeOnModify(inputOfferType: offerType, storageOfferType: maybeOfferType);

                return await this.storageBroker.UpdateOfferTypeAsync(offerType);
            });

        public ValueTask<OfferType> RemoveOfferTypeByIdAsync(Guid offerTypeId) =>
           TryCatch(async () =>
           {
               ValidateOfferTypeId(offerTypeId);

               OfferType maybeOfferType =
                   await this.storageBroker.SelectOfferTypeByIdAsync(offerTypeId);

               ValidateStorageOfferType(maybeOfferType, offerTypeId);

               return await this.storageBroker.DeleteOfferTypeAsync(maybeOfferType);
           });
    }
}