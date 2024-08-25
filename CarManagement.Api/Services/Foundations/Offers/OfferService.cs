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
using CarManagement.Api.Models.Offers;

namespace CarManagement.Api.Services.Foundations.Offers
{
    public partial class OfferService : IOfferService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public OfferService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Offer> AddOfferAsync(Offer offer) =>
        TryCatch(async () =>
        {
            ValidateOfferOnAdd(offer);

            return await this.storageBroker.InsertOfferAsync(offer);
        });

        public IQueryable<Offer> RetrieveAllOffers() =>
            TryCatch(() => this.storageBroker.SelectAllOffers());

        public ValueTask<Offer> RetrieveOfferByIdAsync(Guid offerId) =>
           TryCatch(async () =>
           {
               ValidateOfferId(offerId);

               Offer maybeOffer =
                   await storageBroker.SelectOfferByIdAsync(offerId);

               ValidateStorageOffer(maybeOffer, offerId);

               return maybeOffer;
           });

        public ValueTask<Offer> ModifyOfferAsync(Offer offer) =>
            TryCatch(async () =>
            {
                ValidateOfferOnModify(offer);

                Offer maybeOffer =
                    await this.storageBroker.SelectOfferByIdAsync(offer.Id);

                ValidateAgainstStorageOfferOnModify(inputOffer: offer, storageOffer: maybeOffer);

                return await this.storageBroker.UpdateOfferAsync(offer);
            });

        public ValueTask<Offer> RemoveOfferByIdAsync(Guid offerId) =>
           TryCatch(async () =>
           {
               ValidateOfferId(offerId);

               Offer maybeOffer =
                   await this.storageBroker.SelectOfferByIdAsync(offerId);

               ValidateStorageOffer(maybeOffer, offerId);

               return await this.storageBroker.DeleteOfferAsync(maybeOffer);
           });
    }
}