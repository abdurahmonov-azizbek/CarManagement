// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;
using CarManagement.Api.Models.ServiceTypes.Exceptions;
using CarManagement.Api.Services.Foundations.ServiceTypes;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTypesController : RESTFulController
    {
        private readonly IServiceTypeService serviceTypeService;

        public ServiceTypesController(IServiceTypeService serviceTypeService) =>
            this.serviceTypeService = serviceTypeService;

        [HttpPost]
        public async ValueTask<ActionResult<ServiceType>> PostServiceTypeAsync(ServiceType serviceType)
        {
            try
            {
                ServiceType addedServiceType = await this.serviceTypeService.AddServiceTypeAsync(serviceType);

                return Created(addedServiceType);
            }
            catch (ServiceTypeValidationException serviceTypeValidationException)
            {
                return BadRequest(serviceTypeValidationException.InnerException);
            }
            catch (ServiceTypeDependencyValidationException serviceTypeDependencyValidationException)
                when (serviceTypeDependencyValidationException.InnerException is AlreadyExistsServiceTypeException)
            {
                return Conflict(serviceTypeDependencyValidationException.InnerException);
            }
            catch (ServiceTypeDependencyException serviceTypeDependencyException)
            {
                return InternalServerError(serviceTypeDependencyException.InnerException);
            }
            catch (ServiceTypeServiceException serviceTypeServiceException)
            {
                return InternalServerError(serviceTypeServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<ServiceType>> GetAllServiceTypes()
        {
            try
            {
                IQueryable<ServiceType> allServiceTypes = this.serviceTypeService.RetrieveAllServiceTypes();

                return Ok(allServiceTypes);
            }
            catch (ServiceTypeDependencyException serviceTypeDependencyException)
            {
                return InternalServerError(serviceTypeDependencyException.InnerException);
            }
            catch (ServiceTypeServiceException serviceTypeServiceException)
            {
                return InternalServerError(serviceTypeServiceException.InnerException);
            }
        }

        [HttpGet("{serviceTypeId}")]
        public async ValueTask<ActionResult<ServiceType>> GetServiceTypeByIdAsync(Guid serviceTypeId)
        {
            try
            {
                return await this.serviceTypeService.RetrieveServiceTypeByIdAsync(serviceTypeId);
            }
            catch (ServiceTypeDependencyException serviceTypeDependencyException)
            {
                return InternalServerError(serviceTypeDependencyException);
            }
            catch (ServiceTypeValidationException serviceTypeValidationException)
                when (serviceTypeValidationException.InnerException is InvalidServiceTypeException)
            {
                return BadRequest(serviceTypeValidationException.InnerException);
            }
            catch (ServiceTypeValidationException serviceTypeValidationException)
                 when (serviceTypeValidationException.InnerException is NotFoundServiceTypeException)
            {
                return NotFound(serviceTypeValidationException.InnerException);
            }
            catch (ServiceTypeServiceException serviceTypeServiceException)
            {
                return InternalServerError(serviceTypeServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<ServiceType>> PutServiceTypeAsync(ServiceType serviceType)
        {
            try
            {
                ServiceType modifiedServiceType =
                    await this.serviceTypeService.ModifyServiceTypeAsync(serviceType);

                return Ok(modifiedServiceType);
            }
            catch (ServiceTypeValidationException serviceTypeValidationException)
                when (serviceTypeValidationException.InnerException is NotFoundServiceTypeException)
            {
                return NotFound(serviceTypeValidationException.InnerException);
            }
            catch (ServiceTypeValidationException serviceTypeValidationException)
            {
                return BadRequest(serviceTypeValidationException.InnerException);
            }
            catch (ServiceTypeDependencyValidationException serviceTypeDependencyValidationException)
            {
                return BadRequest(serviceTypeDependencyValidationException.InnerException);
            }
            catch (ServiceTypeDependencyException serviceTypeDependencyException)
            {
                return InternalServerError(serviceTypeDependencyException.InnerException);
            }
            catch (ServiceTypeServiceException serviceTypeServiceException)
            {
                return InternalServerError(serviceTypeServiceException.InnerException);
            }
        }

        [HttpDelete("{serviceTypeId}")]
        public async ValueTask<ActionResult<ServiceType>> DeleteServiceTypeByIdAsync(Guid serviceTypeId)
        {
            try
            {
                ServiceType deletedServiceType = await this.serviceTypeService.RemoveServiceTypeByIdAsync(serviceTypeId);

                return Ok(deletedServiceType);
            }
            catch (ServiceTypeValidationException serviceTypeValidationException)
                when (serviceTypeValidationException.InnerException is NotFoundServiceTypeException)
            {
                return NotFound(serviceTypeValidationException.InnerException);
            }
            catch (ServiceTypeValidationException serviceTypeValidationException)
            {
                return BadRequest(serviceTypeValidationException.InnerException);
            }
            catch (ServiceTypeDependencyValidationException serviceTypeDependencyValidationException)
                when (serviceTypeDependencyValidationException.InnerException is LockedServiceTypeException)
            {
                return Locked(serviceTypeDependencyValidationException.InnerException);
            }
            catch (ServiceTypeDependencyValidationException serviceTypeDependencyValidationException)
            {
                return BadRequest(serviceTypeDependencyValidationException.InnerException);
            }
            catch (ServiceTypeDependencyException serviceTypeDependencyException)
            {
                return InternalServerError(serviceTypeDependencyException.InnerException);
            }
            catch (ServiceTypeServiceException serviceTypeServiceException)
            {
                return InternalServerError(serviceTypeServiceException.InnerException);
            }
        }
    }
}