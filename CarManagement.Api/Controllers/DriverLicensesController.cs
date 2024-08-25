// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;
using CarManagement.Api.Models.DriverLicenses.Exceptions;
using CarManagement.Api.Services.Foundations.DriverLicenses;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverLicensesController : RESTFulController
    {
        private readonly IDriverLicenseService driverLicenseService;

        public DriverLicensesController(IDriverLicenseService driverLicenseService) =>
            this.driverLicenseService = driverLicenseService;

        [HttpPost]
        public async ValueTask<ActionResult<DriverLicense>> PostDriverLicenseAsync(DriverLicense driverLicense)
        {
            try
            {
                DriverLicense addedDriverLicense = await this.driverLicenseService.AddDriverLicenseAsync(driverLicense);

                return Created(addedDriverLicense);
            }
            catch (DriverLicenseValidationException driverLicenseValidationException)
            {
                return BadRequest(driverLicenseValidationException.InnerException);
            }
            catch (DriverLicenseDependencyValidationException driverLicenseDependencyValidationException)
                when (driverLicenseDependencyValidationException.InnerException is AlreadyExistsDriverLicenseException)
            {
                return Conflict(driverLicenseDependencyValidationException.InnerException);
            }
            catch (DriverLicenseDependencyException driverLicenseDependencyException)
            {
                return InternalServerError(driverLicenseDependencyException.InnerException);
            }
            catch (DriverLicenseServiceException driverLicenseServiceException)
            {
                return InternalServerError(driverLicenseServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<DriverLicense>> GetAllDriverLicenses()
        {
            try
            {
                IQueryable<DriverLicense> allDriverLicenses = this.driverLicenseService.RetrieveAllDriverLicenses();

                return Ok(allDriverLicenses);
            }
            catch (DriverLicenseDependencyException driverLicenseDependencyException)
            {
                return InternalServerError(driverLicenseDependencyException.InnerException);
            }
            catch (DriverLicenseServiceException driverLicenseServiceException)
            {
                return InternalServerError(driverLicenseServiceException.InnerException);
            }
        }

        [HttpGet("{driverLicenseId}")]
        public async ValueTask<ActionResult<DriverLicense>> GetDriverLicenseByIdAsync(Guid driverLicenseId)
        {
            try
            {
                return await this.driverLicenseService.RetrieveDriverLicenseByIdAsync(driverLicenseId);
            }
            catch (DriverLicenseDependencyException driverLicenseDependencyException)
            {
                return InternalServerError(driverLicenseDependencyException);
            }
            catch (DriverLicenseValidationException driverLicenseValidationException)
                when (driverLicenseValidationException.InnerException is InvalidDriverLicenseException)
            {
                return BadRequest(driverLicenseValidationException.InnerException);
            }
            catch (DriverLicenseValidationException driverLicenseValidationException)
                 when (driverLicenseValidationException.InnerException is NotFoundDriverLicenseException)
            {
                return NotFound(driverLicenseValidationException.InnerException);
            }
            catch (DriverLicenseServiceException driverLicenseServiceException)
            {
                return InternalServerError(driverLicenseServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<DriverLicense>> PutDriverLicenseAsync(DriverLicense driverLicense)
        {
            try
            {
                DriverLicense modifiedDriverLicense =
                    await this.driverLicenseService.ModifyDriverLicenseAsync(driverLicense);

                return Ok(modifiedDriverLicense);
            }
            catch (DriverLicenseValidationException driverLicenseValidationException)
                when (driverLicenseValidationException.InnerException is NotFoundDriverLicenseException)
            {
                return NotFound(driverLicenseValidationException.InnerException);
            }
            catch (DriverLicenseValidationException driverLicenseValidationException)
            {
                return BadRequest(driverLicenseValidationException.InnerException);
            }
            catch (DriverLicenseDependencyValidationException driverLicenseDependencyValidationException)
            {
                return BadRequest(driverLicenseDependencyValidationException.InnerException);
            }
            catch (DriverLicenseDependencyException driverLicenseDependencyException)
            {
                return InternalServerError(driverLicenseDependencyException.InnerException);
            }
            catch (DriverLicenseServiceException driverLicenseServiceException)
            {
                return InternalServerError(driverLicenseServiceException.InnerException);
            }
        }

        [HttpDelete("{driverLicenseId}")]
        public async ValueTask<ActionResult<DriverLicense>> DeleteDriverLicenseByIdAsync(Guid driverLicenseId)
        {
            try
            {
                DriverLicense deletedDriverLicense = await this.driverLicenseService.RemoveDriverLicenseByIdAsync(driverLicenseId);

                return Ok(deletedDriverLicense);
            }
            catch (DriverLicenseValidationException driverLicenseValidationException)
                when (driverLicenseValidationException.InnerException is NotFoundDriverLicenseException)
            {
                return NotFound(driverLicenseValidationException.InnerException);
            }
            catch (DriverLicenseValidationException driverLicenseValidationException)
            {
                return BadRequest(driverLicenseValidationException.InnerException);
            }
            catch (DriverLicenseDependencyValidationException driverLicenseDependencyValidationException)
                when (driverLicenseDependencyValidationException.InnerException is LockedDriverLicenseException)
            {
                return Locked(driverLicenseDependencyValidationException.InnerException);
            }
            catch (DriverLicenseDependencyValidationException driverLicenseDependencyValidationException)
            {
                return BadRequest(driverLicenseDependencyValidationException.InnerException);
            }
            catch (DriverLicenseDependencyException driverLicenseDependencyException)
            {
                return InternalServerError(driverLicenseDependencyException.InnerException);
            }
            catch (DriverLicenseServiceException driverLicenseServiceException)
            {
                return InternalServerError(driverLicenseServiceException.InnerException);
            }
        }
    }
}