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
using CarManagement.Api.Models.CarModels;

namespace CarManagement.Api.Services.Foundations.CarModels
{
    public partial class CarModelService : ICarModelService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public CarModelService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<CarModel> AddCarModelAsync(CarModel carModel) =>
        TryCatch(async () =>
        {
            ValidateCarModelOnAdd(carModel);

            return await this.storageBroker.InsertCarModelAsync(carModel);
        });

        public IQueryable<CarModel> RetrieveAllCarModels() =>
            TryCatch(() => this.storageBroker.SelectAllCarModels());

        public ValueTask<CarModel> RetrieveCarModelByIdAsync(Guid carModelId) =>
           TryCatch(async () =>
           {
               ValidateCarModelId(carModelId);

               CarModel maybeCarModel =
                   await storageBroker.SelectCarModelByIdAsync(carModelId);

               ValidateStorageCarModel(maybeCarModel, carModelId);

               return maybeCarModel;
           });

        public ValueTask<CarModel> ModifyCarModelAsync(CarModel carModel) =>
            TryCatch(async () =>
            {
                ValidateCarModelOnModify(carModel);

                CarModel maybeCarModel =
                    await this.storageBroker.SelectCarModelByIdAsync(carModel.Id);

                ValidateAgainstStorageCarModelOnModify(inputCarModel: carModel, storageCarModel: maybeCarModel);

                return await this.storageBroker.UpdateCarModelAsync(carModel);
            });

        public ValueTask<CarModel> RemoveCarModelByIdAsync(Guid carModelId) =>
           TryCatch(async () =>
           {
               ValidateCarModelId(carModelId);

               CarModel maybeCarModel =
                   await this.storageBroker.SelectCarModelByIdAsync(carModelId);

               ValidateStorageCarModel(maybeCarModel, carModelId);

               return await this.storageBroker.DeleteCarModelAsync(maybeCarModel);
           });
    }
}