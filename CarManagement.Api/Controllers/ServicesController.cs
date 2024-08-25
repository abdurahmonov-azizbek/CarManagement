// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;
using CarManagement.Api.Models.Services.Exceptions;
using CarManagement.Api.Services.Foundations.Services;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : RESTFulController
    {
        private readonly IServiceService serviceService;

        public ServicesController(IServiceService serviceService) =>
            this.serviceService = serviceService;

        [HttpPost]
        public async ValueTask<ActionResult<Service>> PostServiceAsync(Service service)
        {
            try
            {
                Service addedService = await this.serviceService.AddServiceAsync(service);

                return Created(addedService);
            }
            catch (ServiceValidationException serviceValidationException)
            {
                return BadRequest(serviceValidationException.InnerException);
            }
            catch (ServiceDependencyValidationException serviceDependencyValidationException)
                when (serviceDependencyValidationException.InnerException is AlreadyExistsServiceException)
            {
                return Conflict(serviceDependencyValidationException.InnerException);
            }
            catch (ServiceDependencyException serviceDependencyException)
            {
                return InternalServerError(serviceDependencyException.InnerException);
            }
            catch (ServiceServiceException serviceServiceException)
            {
                return InternalServerError(serviceServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Service>> GetAllServices()
        {
            try
            {
                IQueryable<Service> allServices = this.serviceService.RetrieveAllServices();

                return Ok(allServices);
            }
            catch (ServiceDependencyException serviceDependencyException)
            {
                return InternalServerError(serviceDependencyException.InnerException);
            }
            catch (ServiceServiceException serviceServiceException)
            {
                return InternalServerError(serviceServiceException.InnerException);
            }
        }

        [HttpGet("{serviceId}")]
        public async ValueTask<ActionResult<Service>> GetServiceByIdAsync(Guid serviceId)
        {
            try
            {
                return await this.serviceService.RetrieveServiceByIdAsync(serviceId);
            }
            catch (ServiceDependencyException serviceDependencyException)
            {
                return InternalServerError(serviceDependencyException);
            }
            catch (ServiceValidationException serviceValidationException)
                when (serviceValidationException.InnerException is InvalidServiceException)
            {
                return BadRequest(serviceValidationException.InnerException);
            }
            catch (ServiceValidationException serviceValidationException)
                 when (serviceValidationException.InnerException is NotFoundServiceException)
            {
                return NotFound(serviceValidationException.InnerException);
            }
            catch (ServiceServiceException serviceServiceException)
            {
                return InternalServerError(serviceServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Service>> PutServiceAsync(Service service)
        {
            try
            {
                Service modifiedService =
                    await this.serviceService.ModifyServiceAsync(service);

                return Ok(modifiedService);
            }
            catch (ServiceValidationException serviceValidationException)
                when (serviceValidationException.InnerException is NotFoundServiceException)
            {
                return NotFound(serviceValidationException.InnerException);
            }
            catch (ServiceValidationException serviceValidationException)
            {
                return BadRequest(serviceValidationException.InnerException);
            }
            catch (ServiceDependencyValidationException serviceDependencyValidationException)
            {
                return BadRequest(serviceDependencyValidationException.InnerException);
            }
            catch (ServiceDependencyException serviceDependencyException)
            {
                return InternalServerError(serviceDependencyException.InnerException);
            }
            catch (ServiceServiceException serviceServiceException)
            {
                return InternalServerError(serviceServiceException.InnerException);
            }
        }

        [HttpDelete("{serviceId}")]
        public async ValueTask<ActionResult<Service>> DeleteServiceByIdAsync(Guid serviceId)
        {
            try
            {
                Service deletedService = await this.serviceService.RemoveServiceByIdAsync(serviceId);

                return Ok(deletedService);
            }
            catch (ServiceValidationException serviceValidationException)
                when (serviceValidationException.InnerException is NotFoundServiceException)
            {
                return NotFound(serviceValidationException.InnerException);
            }
            catch (ServiceValidationException serviceValidationException)
            {
                return BadRequest(serviceValidationException.InnerException);
            }
            catch (ServiceDependencyValidationException serviceDependencyValidationException)
                when (serviceDependencyValidationException.InnerException is LockedServiceException)
            {
                return Locked(serviceDependencyValidationException.InnerException);
            }
            catch (ServiceDependencyValidationException serviceDependencyValidationException)
            {
                return BadRequest(serviceDependencyValidationException.InnerException);
            }
            catch (ServiceDependencyException serviceDependencyException)
            {
                return InternalServerError(serviceDependencyException.InnerException);
            }
            catch (ServiceServiceException serviceServiceException)
            {
                return InternalServerError(serviceServiceException.InnerException);
            }
        }
    }
}