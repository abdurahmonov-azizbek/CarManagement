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
using CarManagement.Api.Models.DriverLicenses;

namespace CarManagement.Api.Services.Foundations.DriverLicenses
{
    public partial class DriverLicenseService : IDriverLicenseService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public DriverLicenseService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<DriverLicense> AddDriverLicenseAsync(DriverLicense driverLicense) =>
        TryCatch(async () =>
        {
            ValidateDriverLicenseOnAdd(driverLicense);

            return await this.storageBroker.InsertDriverLicenseAsync(driverLicense);
        });

        public IQueryable<DriverLicense> RetrieveAllDriverLicenses() =>
            TryCatch(() => this.storageBroker.SelectAllDriverLicenses());

        public ValueTask<DriverLicense> RetrieveDriverLicenseByIdAsync(Guid driverLicenseId) =>
           TryCatch(async () =>
           {
               ValidateDriverLicenseId(driverLicenseId);

               DriverLicense maybeDriverLicense =
                   await storageBroker.SelectDriverLicenseByIdAsync(driverLicenseId);

               ValidateStorageDriverLicense(maybeDriverLicense, driverLicenseId);

               return maybeDriverLicense;
           });

        public ValueTask<DriverLicense> ModifyDriverLicenseAsync(DriverLicense driverLicense) =>
            TryCatch(async () =>
            {
                ValidateDriverLicenseOnModify(driverLicense);

                DriverLicense maybeDriverLicense =
                    await this.storageBroker.SelectDriverLicenseByIdAsync(driverLicense.Id);

                ValidateAgainstStorageDriverLicenseOnModify(inputDriverLicense: driverLicense, storageDriverLicense: maybeDriverLicense);

                return await this.storageBroker.UpdateDriverLicenseAsync(driverLicense);
            });

        public ValueTask<DriverLicense> RemoveDriverLicenseByIdAsync(Guid driverLicenseId) =>
           TryCatch(async () =>
           {
               ValidateDriverLicenseId(driverLicenseId);

               DriverLicense maybeDriverLicense =
                   await this.storageBroker.SelectDriverLicenseByIdAsync(driverLicenseId);

               ValidateStorageDriverLicense(maybeDriverLicense, driverLicenseId);

               return await this.storageBroker.DeleteDriverLicenseAsync(maybeDriverLicense);
           });
    }
}