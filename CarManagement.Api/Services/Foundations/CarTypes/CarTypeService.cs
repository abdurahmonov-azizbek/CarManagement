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
using CarManagement.Api.Models.CarTypes;

namespace CarManagement.Api.Services.Foundations.CarTypes
{
    public partial class CarTypeService : ICarTypeService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public CarTypeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<CarType> AddCarTypeAsync(CarType carType) =>
        TryCatch(async () =>
        {
            ValidateCarTypeOnAdd(carType);

            return await this.storageBroker.InsertCarTypeAsync(carType);
        });

        public IQueryable<CarType> RetrieveAllCarTypes() =>
            TryCatch(() => this.storageBroker.SelectAllCarTypes());

        public ValueTask<CarType> RetrieveCarTypeByIdAsync(Guid carTypeId) =>
           TryCatch(async () =>
           {
               ValidateCarTypeId(carTypeId);

               CarType maybeCarType =
                   await storageBroker.SelectCarTypeByIdAsync(carTypeId);

               ValidateStorageCarType(maybeCarType, carTypeId);

               return maybeCarType;
           });

        public ValueTask<CarType> ModifyCarTypeAsync(CarType carType) =>
            TryCatch(async () =>
            {
                ValidateCarTypeOnModify(carType);

                CarType maybeCarType =
                    await this.storageBroker.SelectCarTypeByIdAsync(carType.Id);

                ValidateAgainstStorageCarTypeOnModify(inputCarType: carType, storageCarType: maybeCarType);

                return await this.storageBroker.UpdateCarTypeAsync(carType);
            });

        public ValueTask<CarType> RemoveCarTypeByIdAsync(Guid carTypeId) =>
           TryCatch(async () =>
           {
               ValidateCarTypeId(carTypeId);

               CarType maybeCarType =
                   await this.storageBroker.SelectCarTypeByIdAsync(carTypeId);

               ValidateStorageCarType(maybeCarType, carTypeId);

               return await this.storageBroker.DeleteCarTypeAsync(maybeCarType);
           });
    }
}