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
using CarManagement.Api.Models.ServiceTypes;

namespace CarManagement.Api.Services.Foundations.ServiceTypes
{
    public partial class ServiceTypeService : IServiceTypeService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ServiceTypeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ServiceType> AddServiceTypeAsync(ServiceType serviceType) =>
        TryCatch(async () =>
        {
            ValidateServiceTypeOnAdd(serviceType);

            return await this.storageBroker.InsertServiceTypeAsync(serviceType);
        });

        public IQueryable<ServiceType> RetrieveAllServiceTypes() =>
            TryCatch(() => this.storageBroker.SelectAllServiceTypes());

        public ValueTask<ServiceType> RetrieveServiceTypeByIdAsync(Guid serviceTypeId) =>
           TryCatch(async () =>
           {
               ValidateServiceTypeId(serviceTypeId);

               ServiceType maybeServiceType =
                   await storageBroker.SelectServiceTypeByIdAsync(serviceTypeId);

               ValidateStorageServiceType(maybeServiceType, serviceTypeId);

               return maybeServiceType;
           });

        public ValueTask<ServiceType> ModifyServiceTypeAsync(ServiceType serviceType) =>
            TryCatch(async () =>
            {
                ValidateServiceTypeOnModify(serviceType);

                ServiceType maybeServiceType =
                    await this.storageBroker.SelectServiceTypeByIdAsync(serviceType.Id);

                ValidateAgainstStorageServiceTypeOnModify(inputServiceType: serviceType, storageServiceType: maybeServiceType);

                return await this.storageBroker.UpdateServiceTypeAsync(serviceType);
            });

        public ValueTask<ServiceType> RemoveServiceTypeByIdAsync(Guid serviceTypeId) =>
           TryCatch(async () =>
           {
               ValidateServiceTypeId(serviceTypeId);

               ServiceType maybeServiceType =
                   await this.storageBroker.SelectServiceTypeByIdAsync(serviceTypeId);

               ValidateStorageServiceType(maybeServiceType, serviceTypeId);

               return await this.storageBroker.DeleteServiceTypeAsync(maybeServiceType);
           });
    }
}