// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.OfferTypes;
using CarManagement.Api.Models.OfferTypes.Exceptions;
using CarManagement.Api.Services.Foundations.OfferTypes;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferTypesController : RESTFulController
    {
        private readonly IOfferTypeService offerTypeService;

        public OfferTypesController(IOfferTypeService offerTypeService) =>
            this.offerTypeService = offerTypeService;

        [HttpPost]
        public async ValueTask<ActionResult<OfferType>> PostOfferTypeAsync(OfferType offerType)
        {
            try
            {
                OfferType addedOfferType = await this.offerTypeService.AddOfferTypeAsync(offerType);

                return Created(addedOfferType);
            }
            catch (OfferTypeValidationException offerTypeValidationException)
            {
                return BadRequest(offerTypeValidationException.InnerException);
            }
            catch (OfferTypeDependencyValidationException offerTypeDependencyValidationException)
                when (offerTypeDependencyValidationException.InnerException is AlreadyExistsOfferTypeException)
            {
                return Conflict(offerTypeDependencyValidationException.InnerException);
            }
            catch (OfferTypeDependencyException offerTypeDependencyException)
            {
                return InternalServerError(offerTypeDependencyException.InnerException);
            }
            catch (OfferTypeServiceException offerTypeServiceException)
            {
                return InternalServerError(offerTypeServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<OfferType>> GetAllOfferTypes()
        {
            try
            {
                IQueryable<OfferType> allOfferTypes = this.offerTypeService.RetrieveAllOfferTypes();

                return Ok(allOfferTypes);
            }
            catch (OfferTypeDependencyException offerTypeDependencyException)
            {
                return InternalServerError(offerTypeDependencyException.InnerException);
            }
            catch (OfferTypeServiceException offerTypeServiceException)
            {
                return InternalServerError(offerTypeServiceException.InnerException);
            }
        }

        [HttpGet("{offerTypeId}")]
        public async ValueTask<ActionResult<OfferType>> GetOfferTypeByIdAsync(Guid offerTypeId)
        {
            try
            {
                return await this.offerTypeService.RetrieveOfferTypeByIdAsync(offerTypeId);
            }
            catch (OfferTypeDependencyException offerTypeDependencyException)
            {
                return InternalServerError(offerTypeDependencyException);
            }
            catch (OfferTypeValidationException offerTypeValidationException)
                when (offerTypeValidationException.InnerException is InvalidOfferTypeException)
            {
                return BadRequest(offerTypeValidationException.InnerException);
            }
            catch (OfferTypeValidationException offerTypeValidationException)
                 when (offerTypeValidationException.InnerException is NotFoundOfferTypeException)
            {
                return NotFound(offerTypeValidationException.InnerException);
            }
            catch (OfferTypeServiceException offerTypeServiceException)
            {
                return InternalServerError(offerTypeServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<OfferType>> PutOfferTypeAsync(OfferType offerType)
        {
            try
            {
                OfferType modifiedOfferType =
                    await this.offerTypeService.ModifyOfferTypeAsync(offerType);

                return Ok(modifiedOfferType);
            }
            catch (OfferTypeValidationException offerTypeValidationException)
                when (offerTypeValidationException.InnerException is NotFoundOfferTypeException)
            {
                return NotFound(offerTypeValidationException.InnerException);
            }
            catch (OfferTypeValidationException offerTypeValidationException)
            {
                return BadRequest(offerTypeValidationException.InnerException);
            }
            catch (OfferTypeDependencyValidationException offerTypeDependencyValidationException)
            {
                return BadRequest(offerTypeDependencyValidationException.InnerException);
            }
            catch (OfferTypeDependencyException offerTypeDependencyException)
            {
                return InternalServerError(offerTypeDependencyException.InnerException);
            }
            catch (OfferTypeServiceException offerTypeServiceException)
            {
                return InternalServerError(offerTypeServiceException.InnerException);
            }
        }

        [HttpDelete("{offerTypeId}")]
        public async ValueTask<ActionResult<OfferType>> DeleteOfferTypeByIdAsync(Guid offerTypeId)
        {
            try
            {
                OfferType deletedOfferType = await this.offerTypeService.RemoveOfferTypeByIdAsync(offerTypeId);

                return Ok(deletedOfferType);
            }
            catch (OfferTypeValidationException offerTypeValidationException)
                when (offerTypeValidationException.InnerException is NotFoundOfferTypeException)
            {
                return NotFound(offerTypeValidationException.InnerException);
            }
            catch (OfferTypeValidationException offerTypeValidationException)
            {
                return BadRequest(offerTypeValidationException.InnerException);
            }
            catch (OfferTypeDependencyValidationException offerTypeDependencyValidationException)
                when (offerTypeDependencyValidationException.InnerException is LockedOfferTypeException)
            {
                return Locked(offerTypeDependencyValidationException.InnerException);
            }
            catch (OfferTypeDependencyValidationException offerTypeDependencyValidationException)
            {
                return BadRequest(offerTypeDependencyValidationException.InnerException);
            }
            catch (OfferTypeDependencyException offerTypeDependencyException)
            {
                return InternalServerError(offerTypeDependencyException.InnerException);
            }
            catch (OfferTypeServiceException offerTypeServiceException)
            {
                return InternalServerError(offerTypeServiceException.InnerException);
            }
        }
    }
}