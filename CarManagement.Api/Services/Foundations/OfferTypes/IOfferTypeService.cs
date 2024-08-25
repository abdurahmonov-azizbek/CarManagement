// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;

namespace CarManagement.Api.Services.Foundations.OfferTypes
{
    public interface IOfferTypeService  
    {
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeValidationException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeDependencyValidationException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeDependencyException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeServiceException"></exception>
        ValueTask<OfferType> AddOfferTypeAsync(OfferType offerType);

        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeDependencyException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeServiceException"></exception>
        IQueryable<OfferType> RetrieveAllOfferTypes();

        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeDependencyException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeServiceException"></exception>
        ValueTask<OfferType> RetrieveOfferTypeByIdAsync(Guid offerTypeId);

        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeValidationException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeDependencyValidationException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeDependencyException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeServiceException"></exception>
        ValueTask<OfferType> ModifyOfferTypeAsync(OfferType offerType);

        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeDependencyValidationException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeDependencyException"></exception>
        /// <exception cref="Models.OfferTypes.Exceptions.OfferTypeServiceException"></exception>
        ValueTask<OfferType> RemoveOfferTypeByIdAsync(Guid offerTypeId);
    }
}