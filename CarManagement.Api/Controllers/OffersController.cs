// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Offers;
using CarManagement.Api.Models.Offers.Exceptions;
using CarManagement.Api.Services.Foundations.Offers;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController : RESTFulController
    {
        private readonly IOfferService offerService;

        public OffersController(IOfferService offerService) =>
            this.offerService = offerService;

        [HttpPost]
        public async ValueTask<ActionResult<Offer>> PostOfferAsync(Offer offer)
        {
            try
            {
                Offer addedOffer = await this.offerService.AddOfferAsync(offer);

                return Created(addedOffer);
            }
            catch (OfferValidationException offerValidationException)
            {
                return BadRequest(offerValidationException.InnerException);
            }
            catch (OfferDependencyValidationException offerDependencyValidationException)
                when (offerDependencyValidationException.InnerException is AlreadyExistsOfferException)
            {
                return Conflict(offerDependencyValidationException.InnerException);
            }
            catch (OfferDependencyException offerDependencyException)
            {
                return InternalServerError(offerDependencyException.InnerException);
            }
            catch (OfferServiceException offerServiceException)
            {
                return InternalServerError(offerServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Offer>> GetAllOffers()
        {
            try
            {
                IQueryable<Offer> allOffers = this.offerService.RetrieveAllOffers();

                return Ok(allOffers);
            }
            catch (OfferDependencyException offerDependencyException)
            {
                return InternalServerError(offerDependencyException.InnerException);
            }
            catch (OfferServiceException offerServiceException)
            {
                return InternalServerError(offerServiceException.InnerException);
            }
        }

        [HttpGet("{offerId}")]
        public async ValueTask<ActionResult<Offer>> GetOfferByIdAsync(Guid offerId)
        {
            try
            {
                return await this.offerService.RetrieveOfferByIdAsync(offerId);
            }
            catch (OfferDependencyException offerDependencyException)
            {
                return InternalServerError(offerDependencyException);
            }
            catch (OfferValidationException offerValidationException)
                when (offerValidationException.InnerException is InvalidOfferException)
            {
                return BadRequest(offerValidationException.InnerException);
            }
            catch (OfferValidationException offerValidationException)
                 when (offerValidationException.InnerException is NotFoundOfferException)
            {
                return NotFound(offerValidationException.InnerException);
            }
            catch (OfferServiceException offerServiceException)
            {
                return InternalServerError(offerServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Offer>> PutOfferAsync(Offer offer)
        {
            try
            {
                Offer modifiedOffer =
                    await this.offerService.ModifyOfferAsync(offer);

                return Ok(modifiedOffer);
            }
            catch (OfferValidationException offerValidationException)
                when (offerValidationException.InnerException is NotFoundOfferException)
            {
                return NotFound(offerValidationException.InnerException);
            }
            catch (OfferValidationException offerValidationException)
            {
                return BadRequest(offerValidationException.InnerException);
            }
            catch (OfferDependencyValidationException offerDependencyValidationException)
            {
                return BadRequest(offerDependencyValidationException.InnerException);
            }
            catch (OfferDependencyException offerDependencyException)
            {
                return InternalServerError(offerDependencyException.InnerException);
            }
            catch (OfferServiceException offerServiceException)
            {
                return InternalServerError(offerServiceException.InnerException);
            }
        }

        [HttpDelete("{offerId}")]
        public async ValueTask<ActionResult<Offer>> DeleteOfferByIdAsync(Guid offerId)
        {
            try
            {
                Offer deletedOffer = await this.offerService.RemoveOfferByIdAsync(offerId);

                return Ok(deletedOffer);
            }
            catch (OfferValidationException offerValidationException)
                when (offerValidationException.InnerException is NotFoundOfferException)
            {
                return NotFound(offerValidationException.InnerException);
            }
            catch (OfferValidationException offerValidationException)
            {
                return BadRequest(offerValidationException.InnerException);
            }
            catch (OfferDependencyValidationException offerDependencyValidationException)
                when (offerDependencyValidationException.InnerException is LockedOfferException)
            {
                return Locked(offerDependencyValidationException.InnerException);
            }
            catch (OfferDependencyValidationException offerDependencyValidationException)
            {
                return BadRequest(offerDependencyValidationException.InnerException);
            }
            catch (OfferDependencyException offerDependencyException)
            {
                return InternalServerError(offerDependencyException.InnerException);
            }
            catch (OfferServiceException offerServiceException)
            {
                return InternalServerError(offerServiceException.InnerException);
            }
        }
    }
}