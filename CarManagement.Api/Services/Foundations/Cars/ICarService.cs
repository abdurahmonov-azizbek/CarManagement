// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Cars;

namespace CarManagement.Api.Services.Foundations.Cars
{
    public interface ICarService  
    {
        /// <exception cref="Models.Cars.Exceptions.CarValidationException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarDependencyValidationException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarDependencyException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarServiceException"></exception>
        ValueTask<Car> AddCarAsync(Car car);

        /// <exception cref="Models.Cars.Exceptions.CarDependencyException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarServiceException"></exception>
        IQueryable<Car> RetrieveAllCars();

        /// <exception cref="Models.Cars.Exceptions.CarDependencyException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarServiceException"></exception>
        ValueTask<Car> RetrieveCarByIdAsync(Guid carId);

        /// <exception cref="Models.Cars.Exceptions.CarValidationException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarDependencyValidationException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarDependencyException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarServiceException"></exception>
        ValueTask<Car> ModifyCarAsync(Car car);

        /// <exception cref="Models.Cars.Exceptions.CarDependencyValidationException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarDependencyException"></exception>
        /// <exception cref="Models.Cars.Exceptions.CarServiceException"></exception>
        ValueTask<Car> RemoveCarByIdAsync(Guid carId);
    }
}