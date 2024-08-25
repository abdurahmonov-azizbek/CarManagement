// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Offers;

namespace CarManagement.Api.Services.Foundations.Offers
{
    public interface IOfferService  
    {
        /// <exception cref="Models.Offers.Exceptions.OfferValidationException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferDependencyValidationException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferDependencyException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferServiceException"></exception>
        ValueTask<Offer> AddOfferAsync(Offer offer);

        /// <exception cref="Models.Offers.Exceptions.OfferDependencyException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferServiceException"></exception>
        IQueryable<Offer> RetrieveAllOffers();

        /// <exception cref="Models.Offers.Exceptions.OfferDependencyException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferServiceException"></exception>
        ValueTask<Offer> RetrieveOfferByIdAsync(Guid offerId);

        /// <exception cref="Models.Offers.Exceptions.OfferValidationException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferDependencyValidationException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferDependencyException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferServiceException"></exception>
        ValueTask<Offer> ModifyOfferAsync(Offer offer);

        /// <exception cref="Models.Offers.Exceptions.OfferDependencyValidationException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferDependencyException"></exception>
        /// <exception cref="Models.Offers.Exceptions.OfferServiceException"></exception>
        ValueTask<Offer> RemoveOfferByIdAsync(Guid offerId);
    }
}