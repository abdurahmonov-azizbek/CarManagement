// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;
using CarManagement.Api.Models.CarModels.Exceptions;
using CarManagement.Api.Services.Foundations.CarModels;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarModelsController : RESTFulController
    {
        private readonly ICarModelService carModelService;

        public CarModelsController(ICarModelService carModelService) =>
            this.carModelService = carModelService;

        [HttpPost]
        public async ValueTask<ActionResult<CarModel>> PostCarModelAsync(CarModel carModel)
        {
            try
            {
                CarModel addedCarModel = await this.carModelService.AddCarModelAsync(carModel);

                return Created(addedCarModel);
            }
            catch (CarModelValidationException carModelValidationException)
            {
                return BadRequest(carModelValidationException.InnerException);
            }
            catch (CarModelDependencyValidationException carModelDependencyValidationException)
                when (carModelDependencyValidationException.InnerException is AlreadyExistsCarModelException)
            {
                return Conflict(carModelDependencyValidationException.InnerException);
            }
            catch (CarModelDependencyException carModelDependencyException)
            {
                return InternalServerError(carModelDependencyException.InnerException);
            }
            catch (CarModelServiceException carModelServiceException)
            {
                return InternalServerError(carModelServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<CarModel>> GetAllCarModels()
        {
            try
            {
                IQueryable<CarModel> allCarModels = this.carModelService.RetrieveAllCarModels();

                return Ok(allCarModels);
            }
            catch (CarModelDependencyException carModelDependencyException)
            {
                return InternalServerError(carModelDependencyException.InnerException);
            }
            catch (CarModelServiceException carModelServiceException)
            {
                return InternalServerError(carModelServiceException.InnerException);
            }
        }

        [HttpGet("{carModelId}")]
        public async ValueTask<ActionResult<CarModel>> GetCarModelByIdAsync(Guid carModelId)
        {
            try
            {
                return await this.carModelService.RetrieveCarModelByIdAsync(carModelId);
            }
            catch (CarModelDependencyException carModelDependencyException)
            {
                return InternalServerError(carModelDependencyException);
            }
            catch (CarModelValidationException carModelValidationException)
                when (carModelValidationException.InnerException is InvalidCarModelException)
            {
                return BadRequest(carModelValidationException.InnerException);
            }
            catch (CarModelValidationException carModelValidationException)
                 when (carModelValidationException.InnerException is NotFoundCarModelException)
            {
                return NotFound(carModelValidationException.InnerException);
            }
            catch (CarModelServiceException carModelServiceException)
            {
                return InternalServerError(carModelServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<CarModel>> PutCarModelAsync(CarModel carModel)
        {
            try
            {
                CarModel modifiedCarModel =
                    await this.carModelService.ModifyCarModelAsync(carModel);

                return Ok(modifiedCarModel);
            }
            catch (CarModelValidationException carModelValidationException)
                when (carModelValidationException.InnerException is NotFoundCarModelException)
            {
                return NotFound(carModelValidationException.InnerException);
            }
            catch (CarModelValidationException carModelValidationException)
            {
                return BadRequest(carModelValidationException.InnerException);
            }
            catch (CarModelDependencyValidationException carModelDependencyValidationException)
            {
                return BadRequest(carModelDependencyValidationException.InnerException);
            }
            catch (CarModelDependencyException carModelDependencyException)
            {
                return InternalServerError(carModelDependencyException.InnerException);
            }
            catch (CarModelServiceException carModelServiceException)
            {
                return InternalServerError(carModelServiceException.InnerException);
            }
        }

        [HttpDelete("{carModelId}")]
        public async ValueTask<ActionResult<CarModel>> DeleteCarModelByIdAsync(Guid carModelId)
        {
            try
            {
                CarModel deletedCarModel = await this.carModelService.RemoveCarModelByIdAsync(carModelId);

                return Ok(deletedCarModel);
            }
            catch (CarModelValidationException carModelValidationException)
                when (carModelValidationException.InnerException is NotFoundCarModelException)
            {
                return NotFound(carModelValidationException.InnerException);
            }
            catch (CarModelValidationException carModelValidationException)
            {
                return BadRequest(carModelValidationException.InnerException);
            }
            catch (CarModelDependencyValidationException carModelDependencyValidationException)
                when (carModelDependencyValidationException.InnerException is LockedCarModelException)
            {
                return Locked(carModelDependencyValidationException.InnerException);
            }
            catch (CarModelDependencyValidationException carModelDependencyValidationException)
            {
                return BadRequest(carModelDependencyValidationException.InnerException);
            }
            catch (CarModelDependencyException carModelDependencyException)
            {
                return InternalServerError(carModelDependencyException.InnerException);
            }
            catch (CarModelServiceException carModelServiceException)
            {
                return InternalServerError(carModelServiceException.InnerException);
            }
        }
    }
}