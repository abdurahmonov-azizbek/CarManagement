// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Cars;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Car> InsertCarAsync(Car car);
        IQueryable<Car> SelectAllCars();
        ValueTask<Car> SelectCarByIdAsync(Guid carId);
        ValueTask<Car> DeleteCarAsync(Car car);
        ValueTask<Car> UpdateCarAsync(Car car);
    }
}