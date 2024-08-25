// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Brokers.DateTimes;
using CarManagement.Api.Brokers.Loggings;
using CarManagement.Api.Brokers.Storages;
using CarManagement.Api.Models.Cars;

namespace CarManagement.Api.Services.Foundations.Cars
{
    public partial class CarService : ICarService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public CarService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Car> AddCarAsync(Car car) =>
        TryCatch(async () =>
        {
            ValidateCarOnAdd(car);

            return await this.storageBroker.InsertCarAsync(car);
        });

        public IQueryable<Car> RetrieveAllCars() =>
            TryCatch(() => this.storageBroker.SelectAllCars());

        public ValueTask<Car> RetrieveCarByIdAsync(Guid carId) =>
           TryCatch(async () =>
           {
               ValidateCarId(carId);

               Car maybeCar =
                   await storageBroker.SelectCarByIdAsync(carId);

               ValidateStorageCar(maybeCar, carId);

               return maybeCar;
           });

        public ValueTask<Car> ModifyCarAsync(Car car) =>
            TryCatch(async () =>
            {
                ValidateCarOnModify(car);

                Car maybeCar =
                    await this.storageBroker.SelectCarByIdAsync(car.Id);

                ValidateAgainstStorageCarOnModify(inputCar: car, storageCar: maybeCar);

                return await this.storageBroker.UpdateCarAsync(car);
            });

        public ValueTask<Car> RemoveCarByIdAsync(Guid carId) =>
           TryCatch(async () =>
           {
               ValidateCarId(carId);

               Car maybeCar =
                   await this.storageBroker.SelectCarByIdAsync(carId);

               ValidateStorageCar(maybeCar, carId);

               return await this.storageBroker.DeleteCarAsync(maybeCar);
           });
    }
}