// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Cars;
using CarManagement.Api.Models.Cars.Exceptions;
using CarManagement.Api.Services.Foundations.Cars;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : RESTFulController
    {
        private readonly ICarService carService;

        public CarsController(ICarService carService) =>
            this.carService = carService;

        [HttpPost]
        public async ValueTask<ActionResult<Car>> PostCarAsync(Car car)
        {
            try
            {
                Car addedCar = await this.carService.AddCarAsync(car);

                return Created(addedCar);
            }
            catch (CarValidationException carValidationException)
            {
                return BadRequest(carValidationException.InnerException);
            }
            catch (CarDependencyValidationException carDependencyValidationException)
                when (carDependencyValidationException.InnerException is AlreadyExistsCarException)
            {
                return Conflict(carDependencyValidationException.InnerException);
            }
            catch (CarDependencyException carDependencyException)
            {
                return InternalServerError(carDependencyException.InnerException);
            }
            catch (CarServiceException carServiceException)
            {
                return InternalServerError(carServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Car>> GetAllCars()
        {
            try
            {
                IQueryable<Car> allCars = this.carService.RetrieveAllCars();

                return Ok(allCars);
            }
            catch (CarDependencyException carDependencyException)
            {
                return InternalServerError(carDependencyException.InnerException);
            }
            catch (CarServiceException carServiceException)
            {
                return InternalServerError(carServiceException.InnerException);
            }
        }

        [HttpGet("{carId}")]
        public async ValueTask<ActionResult<Car>> GetCarByIdAsync(Guid carId)
        {
            try
            {
                return await this.carService.RetrieveCarByIdAsync(carId);
            }
            catch (CarDependencyException carDependencyException)
            {
                return InternalServerError(carDependencyException);
            }
            catch (CarValidationException carValidationException)
                when (carValidationException.InnerException is InvalidCarException)
            {
                return BadRequest(carValidationException.InnerException);
            }
            catch (CarValidationException carValidationException)
                 when (carValidationException.InnerException is NotFoundCarException)
            {
                return NotFound(carValidationException.InnerException);
            }
            catch (CarServiceException carServiceException)
            {
                return InternalServerError(carServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Car>> PutCarAsync(Car car)
        {
            try
            {
                Car modifiedCar =
                    await this.carService.ModifyCarAsync(car);

                return Ok(modifiedCar);
            }
            catch (CarValidationException carValidationException)
                when (carValidationException.InnerException is NotFoundCarException)
            {
                return NotFound(carValidationException.InnerException);
            }
            catch (CarValidationException carValidationException)
            {
                return BadRequest(carValidationException.InnerException);
            }
            catch (CarDependencyValidationException carDependencyValidationException)
            {
                return BadRequest(carDependencyValidationException.InnerException);
            }
            catch (CarDependencyException carDependencyException)
            {
                return InternalServerError(carDependencyException.InnerException);
            }
            catch (CarServiceException carServiceException)
            {
                return InternalServerError(carServiceException.InnerException);
            }
        }

        [HttpDelete("{carId}")]
        public async ValueTask<ActionResult<Car>> DeleteCarByIdAsync(Guid carId)
        {
            try
            {
                Car deletedCar = await this.carService.RemoveCarByIdAsync(carId);

                return Ok(deletedCar);
            }
            catch (CarValidationException carValidationException)
                when (carValidationException.InnerException is NotFoundCarException)
            {
                return NotFound(carValidationException.InnerException);
            }
            catch (CarValidationException carValidationException)
            {
                return BadRequest(carValidationException.InnerException);
            }
            catch (CarDependencyValidationException carDependencyValidationException)
                when (carDependencyValidationException.InnerException is LockedCarException)
            {
                return Locked(carDependencyValidationException.InnerException);
            }
            catch (CarDependencyValidationException carDependencyValidationException)
            {
                return BadRequest(carDependencyValidationException.InnerException);
            }
            catch (CarDependencyException carDependencyException)
            {
                return InternalServerError(carDependencyException.InnerException);
            }
            catch (CarServiceException carServiceException)
            {
                return InternalServerError(carServiceException.InnerException);
            }
        }
    }
}