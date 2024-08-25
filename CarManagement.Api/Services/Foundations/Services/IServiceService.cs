// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;

namespace CarManagement.Api.Services.Foundations.Services
{
    public interface IServiceService  
    {
        /// <exception cref="Models.Services.Exceptions.ServiceValidationException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceDependencyValidationException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceDependencyException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceServiceException"></exception>
        ValueTask<Service> AddServiceAsync(Service service);

        /// <exception cref="Models.Services.Exceptions.ServiceDependencyException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceServiceException"></exception>
        IQueryable<Service> RetrieveAllServices();

        /// <exception cref="Models.Services.Exceptions.ServiceDependencyException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceServiceException"></exception>
        ValueTask<Service> RetrieveServiceByIdAsync(Guid serviceId);

        /// <exception cref="Models.Services.Exceptions.ServiceValidationException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceDependencyValidationException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceDependencyException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceServiceException"></exception>
        ValueTask<Service> ModifyServiceAsync(Service service);

        /// <exception cref="Models.Services.Exceptions.ServiceDependencyValidationException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceDependencyException"></exception>
        /// <exception cref="Models.Services.Exceptions.ServiceServiceException"></exception>
        ValueTask<Service> RemoveServiceByIdAsync(Guid serviceId);
    }
}