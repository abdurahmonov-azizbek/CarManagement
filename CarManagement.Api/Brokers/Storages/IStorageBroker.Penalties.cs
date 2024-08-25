// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Penalties;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Penalty> InsertPenaltyAsync(Penalty penalty);
        IQueryable<Penalty> SelectAllPenalties();
        ValueTask<Penalty> SelectPenaltyByIdAsync(Guid penaltyId);
        ValueTask<Penalty> DeletePenaltyAsync(Penalty penalty);
        ValueTask<Penalty> UpdatePenaltyAsync(Penalty penalty);
    }
}