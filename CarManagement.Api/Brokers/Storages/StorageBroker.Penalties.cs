// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Penalties;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Penalty> Penalties { get; set; }

        public async ValueTask<Penalty> InsertPenaltyAsync(Penalty penalty) =>
            await InsertAsync(penalty);

        public IQueryable<Penalty> SelectAllPenalties() =>
            SelectAll<Penalty>();

        public async ValueTask<Penalty> SelectPenaltyByIdAsync(Guid penaltyId) =>
            await SelectAsync<Penalty>(penaltyId);

        public async ValueTask<Penalty> DeletePenaltyAsync(Penalty penalty) =>
            await DeleteAsync(penalty);

        public async ValueTask<Penalty> UpdatePenaltyAsync(Penalty penalty) =>
            await UpdateAsync(penalty);
    }
}