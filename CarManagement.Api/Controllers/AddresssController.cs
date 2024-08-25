// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Addresss;
using CarManagement.Api.Models.Addresss.Exceptions;
using CarManagement.Api.Services.Foundations.Addresss;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddresssController : RESTFulController
    {
        private readonly IAddressService addressService;

        public AddresssController(IAddressService addressService) =>
            this.addressService = addressService;

        [HttpPost]
        public async ValueTask<ActionResult<Address>> PostAddressAsync(Address address)
        {
            try
            {
                Address addedAddress = await this.addressService.AddAddressAsync(address);

                return Created(addedAddress);
            }
            catch (AddressValidationException addressValidationException)
            {
                return BadRequest(addressValidationException.InnerException);
            }
            catch (AddressDependencyValidationException addressDependencyValidationException)
                when (addressDependencyValidationException.InnerException is AlreadyExistsAddressException)
            {
                return Conflict(addressDependencyValidationException.InnerException);
            }
            catch (AddressDependencyException addressDependencyException)
            {
                return InternalServerError(addressDependencyException.InnerException);
            }
            catch (AddressServiceException addressServiceException)
            {
                return InternalServerError(addressServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Address>> GetAllAddresss()
        {
            try
            {
                IQueryable<Address> allAddresss = this.addressService.RetrieveAllAddresss();

                return Ok(allAddresss);
            }
            catch (AddressDependencyException addressDependencyException)
            {
                return InternalServerError(addressDependencyException.InnerException);
            }
            catch (AddressServiceException addressServiceException)
            {
                return InternalServerError(addressServiceException.InnerException);
            }
        }

        [HttpGet("{addressId}")]
        public async ValueTask<ActionResult<Address>> GetAddressByIdAsync(Guid addressId)
        {
            try
            {
                return await this.addressService.RetrieveAddressByIdAsync(addressId);
            }
            catch (AddressDependencyException addressDependencyException)
            {
                return InternalServerError(addressDependencyException);
            }
            catch (AddressValidationException addressValidationException)
                when (addressValidationException.InnerException is InvalidAddressException)
            {
                return BadRequest(addressValidationException.InnerException);
            }
            catch (AddressValidationException addressValidationException)
                 when (addressValidationException.InnerException is NotFoundAddressException)
            {
                return NotFound(addressValidationException.InnerException);
            }
            catch (AddressServiceException addressServiceException)
            {
                return InternalServerError(addressServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Address>> PutAddressAsync(Address address)
        {
            try
            {
                Address modifiedAddress =
                    await this.addressService.ModifyAddressAsync(address);

                return Ok(modifiedAddress);
            }
            catch (AddressValidationException addressValidationException)
                when (addressValidationException.InnerException is NotFoundAddressException)
            {
                return NotFound(addressValidationException.InnerException);
            }
            catch (AddressValidationException addressValidationException)
            {
                return BadRequest(addressValidationException.InnerException);
            }
            catch (AddressDependencyValidationException addressDependencyValidationException)
            {
                return BadRequest(addressDependencyValidationException.InnerException);
            }
            catch (AddressDependencyException addressDependencyException)
            {
                return InternalServerError(addressDependencyException.InnerException);
            }
            catch (AddressServiceException addressServiceException)
            {
                return InternalServerError(addressServiceException.InnerException);
            }
        }

        [HttpDelete("{addressId}")]
        public async ValueTask<ActionResult<Address>> DeleteAddressByIdAsync(Guid addressId)
        {
            try
            {
                Address deletedAddress = await this.addressService.RemoveAddressByIdAsync(addressId);

                return Ok(deletedAddress);
            }
            catch (AddressValidationException addressValidationException)
                when (addressValidationException.InnerException is NotFoundAddressException)
            {
                return NotFound(addressValidationException.InnerException);
            }
            catch (AddressValidationException addressValidationException)
            {
                return BadRequest(addressValidationException.InnerException);
            }
            catch (AddressDependencyValidationException addressDependencyValidationException)
                when (addressDependencyValidationException.InnerException is LockedAddressException)
            {
                return Locked(addressDependencyValidationException.InnerException);
            }
            catch (AddressDependencyValidationException addressDependencyValidationException)
            {
                return BadRequest(addressDependencyValidationException.InnerException);
            }
            catch (AddressDependencyException addressDependencyException)
            {
                return InternalServerError(addressDependencyException.InnerException);
            }
            catch (AddressServiceException addressServiceException)
            {
                return InternalServerError(addressServiceException.InnerException);
            }
        }
    }
}