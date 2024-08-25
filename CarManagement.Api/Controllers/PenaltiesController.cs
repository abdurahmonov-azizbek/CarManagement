// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Penalties;
using CarManagement.Api.Models.Penalties.Exceptions;
using CarManagement.Api.Services.Foundations.Penalties;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace CarManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenaltiesController : RESTFulController
    {
        private readonly IPenaltyService penaltyService;

        public PenaltiesController(IPenaltyService penaltyService) =>
            this.penaltyService = penaltyService;

        [HttpPost]
        public async ValueTask<ActionResult<Penalty>> PostPenaltyAsync(Penalty penalty)
        {
            try
            {
                Penalty addedPenalty = await this.penaltyService.AddPenaltyAsync(penalty);

                return Created(addedPenalty);
            }
            catch (PenaltyValidationException penaltyValidationException)
            {
                return BadRequest(penaltyValidationException.InnerException);
            }
            catch (PenaltyDependencyValidationException penaltyDependencyValidationException)
                when (penaltyDependencyValidationException.InnerException is AlreadyExistsPenaltyException)
            {
                return Conflict(penaltyDependencyValidationException.InnerException);
            }
            catch (PenaltyDependencyException penaltyDependencyException)
            {
                return InternalServerError(penaltyDependencyException.InnerException);
            }
            catch (PenaltyServiceException penaltyServiceException)
            {
                return InternalServerError(penaltyServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Penalty>> GetAllPenalties()
        {
            try
            {
                IQueryable<Penalty> allPenalties = this.penaltyService.RetrieveAllPenalties();

                return Ok(allPenalties);
            }
            catch (PenaltyDependencyException penaltyDependencyException)
            {
                return InternalServerError(penaltyDependencyException.InnerException);
            }
            catch (PenaltyServiceException penaltyServiceException)
            {
                return InternalServerError(penaltyServiceException.InnerException);
            }
        }

        [HttpGet("{penaltyId}")]
        public async ValueTask<ActionResult<Penalty>> GetPenaltyByIdAsync(Guid penaltyId)
        {
            try
            {
                return await this.penaltyService.RetrievePenaltyByIdAsync(penaltyId);
            }
            catch (PenaltyDependencyException penaltyDependencyException)
            {
                return InternalServerError(penaltyDependencyException);
            }
            catch (PenaltyValidationException penaltyValidationException)
                when (penaltyValidationException.InnerException is InvalidPenaltyException)
            {
                return BadRequest(penaltyValidationException.InnerException);
            }
            catch (PenaltyValidationException penaltyValidationException)
                 when (penaltyValidationException.InnerException is NotFoundPenaltyException)
            {
                return NotFound(penaltyValidationException.InnerException);
            }
            catch (PenaltyServiceException penaltyServiceException)
            {
                return InternalServerError(penaltyServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Penalty>> PutPenaltyAsync(Penalty penalty)
        {
            try
            {
                Penalty modifiedPenalty =
                    await this.penaltyService.ModifyPenaltyAsync(penalty);

                return Ok(modifiedPenalty);
            }
            catch (PenaltyValidationException penaltyValidationException)
                when (penaltyValidationException.InnerException is NotFoundPenaltyException)
            {
                return NotFound(penaltyValidationException.InnerException);
            }
            catch (PenaltyValidationException penaltyValidationException)
            {
                return BadRequest(penaltyValidationException.InnerException);
            }
            catch (PenaltyDependencyValidationException penaltyDependencyValidationException)
            {
                return BadRequest(penaltyDependencyValidationException.InnerException);
            }
            catch (PenaltyDependencyException penaltyDependencyException)
            {
                return InternalServerError(penaltyDependencyException.InnerException);
            }
            catch (PenaltyServiceException penaltyServiceException)
            {
                return InternalServerError(penaltyServiceException.InnerException);
            }
        }

        [HttpDelete("{penaltyId}")]
        public async ValueTask<ActionResult<Penalty>> DeletePenaltyByIdAsync(Guid penaltyId)
        {
            try
            {
                Penalty deletedPenalty = await this.penaltyService.RemovePenaltyByIdAsync(penaltyId);

                return Ok(deletedPenalty);
            }
            catch (PenaltyValidationException penaltyValidationException)
                when (penaltyValidationException.InnerException is NotFoundPenaltyException)
            {
                return NotFound(penaltyValidationException.InnerException);
            }
            catch (PenaltyValidationException penaltyValidationException)
            {
                return BadRequest(penaltyValidationException.InnerException);
            }
            catch (PenaltyDependencyValidationException penaltyDependencyValidationException)
                when (penaltyDependencyValidationException.InnerException is LockedPenaltyException)
            {
                return Locked(penaltyDependencyValidationException.InnerException);
            }
            catch (PenaltyDependencyValidationException penaltyDependencyValidationException)
            {
                return BadRequest(penaltyDependencyValidationException.InnerException);
            }
            catch (PenaltyDependencyException penaltyDependencyException)
            {
                return InternalServerError(penaltyDependencyException.InnerException);
            }
            catch (PenaltyServiceException penaltyServiceException)
            {
                return InternalServerError(penaltyServiceException.InnerException);
            }
        }
    }
}