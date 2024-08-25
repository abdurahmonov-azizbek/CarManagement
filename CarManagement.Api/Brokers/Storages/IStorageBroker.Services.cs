// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Services;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Service> InsertServiceAsync(Service service);
        IQueryable<Service> SelectAllServices();
        ValueTask<Service> SelectServiceByIdAsync(Guid serviceId);
        ValueTask<Service> DeleteServiceAsync(Service service);
        ValueTask<Service> UpdateServiceAsync(Service service);
    }
}