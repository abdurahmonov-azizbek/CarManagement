// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;

namespace CarManagement.Api.Services.Foundations.CarTypes
{
    public interface ICarTypeService  
    {
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeValidationException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeDependencyValidationException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeDependencyException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeServiceException"></exception>
        ValueTask<CarType> AddCarTypeAsync(CarType carType);

        /// <exception cref="Models.CarTypes.Exceptions.CarTypeDependencyException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeServiceException"></exception>
        IQueryable<CarType> RetrieveAllCarTypes();

        /// <exception cref="Models.CarTypes.Exceptions.CarTypeDependencyException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeServiceException"></exception>
        ValueTask<CarType> RetrieveCarTypeByIdAsync(Guid carTypeId);

        /// <exception cref="Models.CarTypes.Exceptions.CarTypeValidationException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeDependencyValidationException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeDependencyException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeServiceException"></exception>
        ValueTask<CarType> ModifyCarTypeAsync(CarType carType);

        /// <exception cref="Models.CarTypes.Exceptions.CarTypeDependencyValidationException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeDependencyException"></exception>
        /// <exception cref="Models.CarTypes.Exceptions.CarTypeServiceException"></exception>
        ValueTask<CarType> RemoveCarTypeByIdAsync(Guid carTypeId);
    }
}