// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Service> Services { get; set; }

        public async ValueTask<Service> InsertServiceAsync(Service service) =>
            await InsertAsync(service);

        public IQueryable<Service> SelectAllServices() =>
            SelectAll<Service>();

        public async ValueTask<Service> SelectServiceByIdAsync(Guid serviceId) =>
            await SelectAsync<Service>(serviceId);

        public async ValueTask<Service> DeleteServiceAsync(Service service) =>
            await DeleteAsync(service);

        public async ValueTask<Service> UpdateServiceAsync(Service service) =>
            await UpdateAsync(service);
    }
}