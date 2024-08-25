// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;

namespace CarManagement.Api.Services.Foundations.ServiceTypes
{
    public interface IServiceTypeService  
    {
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeValidationException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeDependencyValidationException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeDependencyException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeServiceException"></exception>
        ValueTask<ServiceType> AddServiceTypeAsync(ServiceType serviceType);

        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeDependencyException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeServiceException"></exception>
        IQueryable<ServiceType> RetrieveAllServiceTypes();

        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeDependencyException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeServiceException"></exception>
        ValueTask<ServiceType> RetrieveServiceTypeByIdAsync(Guid serviceTypeId);

        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeValidationException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeDependencyValidationException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeDependencyException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeServiceException"></exception>
        ValueTask<ServiceType> ModifyServiceTypeAsync(ServiceType serviceType);

        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeDependencyValidationException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeDependencyException"></exception>
        /// <exception cref="Models.ServiceTypes.Exceptions.ServiceTypeServiceException"></exception>
        ValueTask<ServiceType> RemoveServiceTypeByIdAsync(Guid serviceTypeId);
    }
}