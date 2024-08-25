// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;
using CarManagement.Api.Models.CarTypes.Exceptions;
using CarManagement.Api.Services.Foundations.CarTypes;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarTypesController : RESTFulController
    {
        private readonly ICarTypeService carTypeService;

        public CarTypesController(ICarTypeService carTypeService) =>
            this.carTypeService = carTypeService;

        [HttpPost]
        public async ValueTask<ActionResult<CarType>> PostCarTypeAsync(CarType carType)
        {
            try
            {
                CarType addedCarType = await this.carTypeService.AddCarTypeAsync(carType);

                return Created(addedCarType);
            }
            catch (CarTypeValidationException carTypeValidationException)
            {
                return BadRequest(carTypeValidationException.InnerException);
            }
            catch (CarTypeDependencyValidationException carTypeDependencyValidationException)
                when (carTypeDependencyValidationException.InnerException is AlreadyExistsCarTypeException)
            {
                return Conflict(carTypeDependencyValidationException.InnerException);
            }
            catch (CarTypeDependencyException carTypeDependencyException)
            {
                return InternalServerError(carTypeDependencyException.InnerException);
            }
            catch (CarTypeServiceException carTypeServiceException)
            {
                return InternalServerError(carTypeServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<CarType>> GetAllCarTypes()
        {
            try
            {
                IQueryable<CarType> allCarTypes = this.carTypeService.RetrieveAllCarTypes();

                return Ok(allCarTypes);
            }
            catch (CarTypeDependencyException carTypeDependencyException)
            {
                return InternalServerError(carTypeDependencyException.InnerException);
            }
            catch (CarTypeServiceException carTypeServiceException)
            {
                return InternalServerError(carTypeServiceException.InnerException);
            }
        }

        [HttpGet("{carTypeId}")]
        public async ValueTask<ActionResult<CarType>> GetCarTypeByIdAsync(Guid carTypeId)
        {
            try
            {
                return await this.carTypeService.RetrieveCarTypeByIdAsync(carTypeId);
            }
            catch (CarTypeDependencyException carTypeDependencyException)
            {
                return InternalServerError(carTypeDependencyException);
            }
            catch (CarTypeValidationException carTypeValidationException)
                when (carTypeValidationException.InnerException is InvalidCarTypeException)
            {
                return BadRequest(carTypeValidationException.InnerException);
            }
            catch (CarTypeValidationException carTypeValidationException)
                 when (carTypeValidationException.InnerException is NotFoundCarTypeException)
            {
                return NotFound(carTypeValidationException.InnerException);
            }
            catch (CarTypeServiceException carTypeServiceException)
            {
                return InternalServerError(carTypeServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<CarType>> PutCarTypeAsync(CarType carType)
        {
            try
            {
                CarType modifiedCarType =
                    await this.carTypeService.ModifyCarTypeAsync(carType);

                return Ok(modifiedCarType);
            }
            catch (CarTypeValidationException carTypeValidationException)
                when (carTypeValidationException.InnerException is NotFoundCarTypeException)
            {
                return NotFound(carTypeValidationException.InnerException);
            }
            catch (CarTypeValidationException carTypeValidationException)
            {
                return BadRequest(carTypeValidationException.InnerException);
            }
            catch (CarTypeDependencyValidationException carTypeDependencyValidationException)
            {
                return BadRequest(carTypeDependencyValidationException.InnerException);
            }
            catch (CarTypeDependencyException carTypeDependencyException)
            {
                return InternalServerError(carTypeDependencyException.InnerException);
            }
            catch (CarTypeServiceException carTypeServiceException)
            {
                return InternalServerError(carTypeServiceException.InnerException);
            }
        }

        [HttpDelete("{carTypeId}")]
        public async ValueTask<ActionResult<CarType>> DeleteCarTypeByIdAsync(Guid carTypeId)
        {
            try
            {
                CarType deletedCarType = await this.carTypeService.RemoveCarTypeByIdAsync(carTypeId);

                return Ok(deletedCarType);
            }
            catch (CarTypeValidationException carTypeValidationException)
                when (carTypeValidationException.InnerException is NotFoundCarTypeException)
            {
                return NotFound(carTypeValidationException.InnerException);
            }
            catch (CarTypeValidationException carTypeValidationException)
            {
                return BadRequest(carTypeValidationException.InnerException);
            }
            catch (CarTypeDependencyValidationException carTypeDependencyValidationException)
                when (carTypeDependencyValidationException.InnerException is LockedCarTypeException)
            {
                return Locked(carTypeDependencyValidationException.InnerException);
            }
            catch (CarTypeDependencyValidationException carTypeDependencyValidationException)
            {
                return BadRequest(carTypeDependencyValidationException.InnerException);
            }
            catch (CarTypeDependencyException carTypeDependencyException)
            {
                return InternalServerError(carTypeDependencyException.InnerException);
            }
            catch (CarTypeServiceException carTypeServiceException)
            {
                return InternalServerError(carTypeServiceException.InnerException);
            }
        }
    }
}