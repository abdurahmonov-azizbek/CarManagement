// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<CarModel> CarModels { get; set; }

        public async ValueTask<CarModel> InsertCarModelAsync(CarModel carModel) =>
            await InsertAsync(carModel);

        public IQueryable<CarModel> SelectAllCarModels() =>
            SelectAll<CarModel>();

        public async ValueTask<CarModel> SelectCarModelByIdAsync(Guid carModelId) =>
            await SelectAsync<CarModel>(carModelId);

        public async ValueTask<CarModel> DeleteCarModelAsync(CarModel carModel) =>
            await DeleteAsync(carModel);

        public async ValueTask<CarModel> UpdateCarModelAsync(CarModel carModel) =>
            await UpdateAsync(carModel);
    }
}