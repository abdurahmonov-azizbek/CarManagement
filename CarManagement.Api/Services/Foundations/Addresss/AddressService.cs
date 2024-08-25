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
using CarManagement.Api.Models.Addresss;

namespace CarManagement.Api.Services.Foundations.Addresss
{
    public partial class AddressService : IAddressService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public AddressService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Address> AddAddressAsync(Address address) =>
        TryCatch(async () =>
        {
            ValidateAddressOnAdd(address);

            return await this.storageBroker.InsertAddressAsync(address);
        });

        public IQueryable<Address> RetrieveAllAddresss() =>
            TryCatch(() => this.storageBroker.SelectAllAddresss());

        public ValueTask<Address> RetrieveAddressByIdAsync(Guid addressId) =>
           TryCatch(async () =>
           {
               ValidateAddressId(addressId);

               Address maybeAddress =
                   await storageBroker.SelectAddressByIdAsync(addressId);

               ValidateStorageAddress(maybeAddress, addressId);

               return maybeAddress;
           });

        public ValueTask<Address> ModifyAddressAsync(Address address) =>
            TryCatch(async () =>
            {
                ValidateAddressOnModify(address);

                Address maybeAddress =
                    await this.storageBroker.SelectAddressByIdAsync(address.Id);

                ValidateAgainstStorageAddressOnModify(inputAddress: address, storageAddress: maybeAddress);

                return await this.storageBroker.UpdateAddressAsync(address);
            });

        public ValueTask<Address> RemoveAddressByIdAsync(Guid addressId) =>
           TryCatch(async () =>
           {
               ValidateAddressId(addressId);

               Address maybeAddress =
                   await this.storageBroker.SelectAddressByIdAsync(addressId);

               ValidateStorageAddress(maybeAddress, addressId);

               return await this.storageBroker.DeleteAddressAsync(maybeAddress);
           });
    }
}