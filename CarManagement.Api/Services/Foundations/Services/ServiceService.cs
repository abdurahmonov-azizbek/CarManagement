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
using CarManagement.Api.Models.Services;

namespace CarManagement.Api.Services.Foundations.Services
{
    public partial class ServiceService : IServiceService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ServiceService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Service> AddServiceAsync(Service service) =>
        TryCatch(async () =>
        {
            ValidateServiceOnAdd(service);

            return await this.storageBroker.InsertServiceAsync(service);
        });

        public IQueryable<Service> RetrieveAllServices() =>
            TryCatch(() => this.storageBroker.SelectAllServices());

        public ValueTask<Service> RetrieveServiceByIdAsync(Guid serviceId) =>
           TryCatch(async () =>
           {
               ValidateServiceId(serviceId);

               Service maybeService =
                   await storageBroker.SelectServiceByIdAsync(serviceId);

               ValidateStorageService(maybeService, serviceId);

               return maybeService;
           });

        public ValueTask<Service> ModifyServiceAsync(Service service) =>
            TryCatch(async () =>
            {
                ValidateServiceOnModify(service);

                Service maybeService =
                    await this.storageBroker.SelectServiceByIdAsync(service.Id);

                ValidateAgainstStorageServiceOnModify(inputService: service, storageService: maybeService);

                return await this.storageBroker.UpdateServiceAsync(service);
            });

        public ValueTask<Service> RemoveServiceByIdAsync(Guid serviceId) =>
           TryCatch(async () =>
           {
               ValidateServiceId(serviceId);

               Service maybeService =
                   await this.storageBroker.SelectServiceByIdAsync(serviceId);

               ValidateStorageService(maybeService, serviceId);

               return await this.storageBroker.DeleteServiceAsync(maybeService);
           });
    }
}