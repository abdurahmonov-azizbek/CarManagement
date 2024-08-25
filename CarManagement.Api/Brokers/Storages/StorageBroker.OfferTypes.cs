// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<OfferType> OfferTypes { get; set; }

        public async ValueTask<OfferType> InsertOfferTypeAsync(OfferType offerType) =>
            await InsertAsync(offerType);

        public IQueryable<OfferType> SelectAllOfferTypes() =>
            SelectAll<OfferType>();

        public async ValueTask<OfferType> SelectOfferTypeByIdAsync(Guid offerTypeId) =>
            await SelectAsync<OfferType>(offerTypeId);

        public async ValueTask<OfferType> DeleteOfferTypeAsync(OfferType offerType) =>
            await DeleteAsync(offerType);

        public async ValueTask<OfferType> UpdateOfferTypeAsync(OfferType offerType) =>
            await UpdateAsync(offerType);
    }
}