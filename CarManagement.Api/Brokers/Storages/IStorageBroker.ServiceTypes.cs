// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.ServiceTypes;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<ServiceType> InsertServiceTypeAsync(ServiceType serviceType);
        IQueryable<ServiceType> SelectAllServiceTypes();
        ValueTask<ServiceType> SelectServiceTypeByIdAsync(Guid serviceTypeId);
        ValueTask<ServiceType> DeleteServiceTypeAsync(ServiceType serviceType);
        ValueTask<ServiceType> UpdateServiceTypeAsync(ServiceType serviceType);
    }
}