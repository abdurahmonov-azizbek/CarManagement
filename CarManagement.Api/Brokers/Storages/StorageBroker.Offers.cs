// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Offers;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Offer> Offers { get; set; }

        public async ValueTask<Offer> InsertOfferAsync(Offer offer) =>
            await InsertAsync(offer);

        public IQueryable<Offer> SelectAllOffers() =>
            SelectAll<Offer>();

        public async ValueTask<Offer> SelectOfferByIdAsync(Guid offerId) =>
            await SelectAsync<Offer>(offerId);

        public async ValueTask<Offer> DeleteOfferAsync(Offer offer) =>
            await DeleteAsync(offer);

        public async ValueTask<Offer> UpdateOfferAsync(Offer offer) =>
            await UpdateAsync(offer);
    }
}