// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<CarType> InsertCarTypeAsync(CarType carType);
        IQueryable<CarType> SelectAllCarTypes();
        ValueTask<CarType> SelectCarTypeByIdAsync(Guid carTypeId);
        ValueTask<CarType> DeleteCarTypeAsync(CarType carType);
        ValueTask<CarType> UpdateCarTypeAsync(CarType carType);
    }
}