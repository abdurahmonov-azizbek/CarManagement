// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<ServiceType> ServiceTypes { get; set; }

        public async ValueTask<ServiceType> InsertServiceTypeAsync(ServiceType serviceType) =>
            await InsertAsync(serviceType);

        public IQueryable<ServiceType> SelectAllServiceTypes() =>
            SelectAll<ServiceType>();

        public async ValueTask<ServiceType> SelectServiceTypeByIdAsync(Guid serviceTypeId) =>
            await SelectAsync<ServiceType>(serviceTypeId);

        public async ValueTask<ServiceType> DeleteServiceTypeAsync(ServiceType serviceType) =>
            await DeleteAsync(serviceType);

        public async ValueTask<ServiceType> UpdateServiceTypeAsync(ServiceType serviceType) =>
            await UpdateAsync(serviceType);
    }
}