// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.CarModels;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<CarModel> InsertCarModelAsync(CarModel carModel);
        IQueryable<CarModel> SelectAllCarModels();
        ValueTask<CarModel> SelectCarModelByIdAsync(Guid carModelId);
        ValueTask<CarModel> DeleteCarModelAsync(CarModel carModel);
        ValueTask<CarModel> UpdateCarModelAsync(CarModel carModel);
    }
}