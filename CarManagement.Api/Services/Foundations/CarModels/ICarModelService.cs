// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;

namespace CarManagement.Api.Services.Foundations.CarModels
{
    public interface ICarModelService  
    {
        /// <exception cref="Models.CarModels.Exceptions.CarModelValidationException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelDependencyValidationException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelDependencyException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelServiceException"></exception>
        ValueTask<CarModel> AddCarModelAsync(CarModel carModel);

        /// <exception cref="Models.CarModels.Exceptions.CarModelDependencyException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelServiceException"></exception>
        IQueryable<CarModel> RetrieveAllCarModels();

        /// <exception cref="Models.CarModels.Exceptions.CarModelDependencyException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelServiceException"></exception>
        ValueTask<CarModel> RetrieveCarModelByIdAsync(Guid carModelId);

        /// <exception cref="Models.CarModels.Exceptions.CarModelValidationException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelDependencyValidationException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelDependencyException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelServiceException"></exception>
        ValueTask<CarModel> ModifyCarModelAsync(CarModel carModel);

        /// <exception cref="Models.CarModels.Exceptions.CarModelDependencyValidationException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelDependencyException"></exception>
        /// <exception cref="Models.CarModels.Exceptions.CarModelServiceException"></exception>
        ValueTask<CarModel> RemoveCarModelByIdAsync(Guid carModelId);
    }
}