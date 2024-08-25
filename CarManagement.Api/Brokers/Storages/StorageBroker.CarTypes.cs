// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarTypes;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<CarType> CarTypes { get; set; }

        public async ValueTask<CarType> InsertCarTypeAsync(CarType carType) =>
            await InsertAsync(carType);

        public IQueryable<CarType> SelectAllCarTypes() =>
            SelectAll<CarType>();

        public async ValueTask<CarType> SelectCarTypeByIdAsync(Guid carTypeId) =>
            await SelectAsync<CarType>(carTypeId);

        public async ValueTask<CarType> DeleteCarTypeAsync(CarType carType) =>
            await DeleteAsync(carType);

        public async ValueTask<CarType> UpdateCarTypeAsync(CarType carType) =>
            await UpdateAsync(carType);
    }
}