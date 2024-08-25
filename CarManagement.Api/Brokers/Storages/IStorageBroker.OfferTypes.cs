// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<OfferType> InsertOfferTypeAsync(OfferType offerType);
        IQueryable<OfferType> SelectAllOfferTypes();
        ValueTask<OfferType> SelectOfferTypeByIdAsync(Guid offerTypeId);
        ValueTask<OfferType> DeleteOfferTypeAsync(OfferType offerType);
        ValueTask<OfferType> UpdateOfferTypeAsync(OfferType offerType);
    }
}