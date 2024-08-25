// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Penalties;

namespace CarManagement.Api.Services.Foundations.Penalties
{
    public interface IPenaltyService  
    {
        /// <exception cref="Models.Penalties.Exceptions.PenaltyValidationException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyDependencyValidationException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyDependencyException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyServiceException"></exception>
        ValueTask<Penalty> AddPenaltyAsync(Penalty penalty);

        /// <exception cref="Models.Penalties.Exceptions.PenaltyDependencyException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyServiceException"></exception>
        IQueryable<Penalty> RetrieveAllPenalties();

        /// <exception cref="Models.Penalties.Exceptions.PenaltyDependencyException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyServiceException"></exception>
        ValueTask<Penalty> RetrievePenaltyByIdAsync(Guid penaltyId);

        /// <exception cref="Models.Penalties.Exceptions.PenaltyValidationException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyDependencyValidationException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyDependencyException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyServiceException"></exception>
        ValueTask<Penalty> ModifyPenaltyAsync(Penalty penalty);

        /// <exception cref="Models.Penalties.Exceptions.PenaltyDependencyValidationException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyDependencyException"></exception>
        /// <exception cref="Models.Penalties.Exceptions.PenaltyServiceException"></exception>
        ValueTask<Penalty> RemovePenaltyByIdAsync(Guid penaltyId);
    }
}