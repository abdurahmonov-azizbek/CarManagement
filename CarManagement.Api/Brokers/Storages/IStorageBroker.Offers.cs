// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Offers;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Offer> InsertOfferAsync(Offer offer);
        IQueryable<Offer> SelectAllOffers();
        ValueTask<Offer> SelectOfferByIdAsync(Guid offerId);
        ValueTask<Offer> DeleteOfferAsync(Offer offer);
        ValueTask<Offer> UpdateOfferAsync(Offer offer);
    }
}