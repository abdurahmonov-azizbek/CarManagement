// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Cars;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Car> Cars { get; set; }

        public async ValueTask<Car> InsertCarAsync(Car car) =>
            await InsertAsync(car);

        public IQueryable<Car> SelectAllCars() =>
            SelectAll<Car>();

        public async ValueTask<Car> SelectCarByIdAsync(Guid carId) =>
            await SelectAsync<Car>(carId);

        public async ValueTask<Car> DeleteCarAsync(Car car) =>
            await DeleteAsync(car);

        public async ValueTask<Car> UpdateCarAsync(Car car) =>
            await UpdateAsync(car);
    }
}